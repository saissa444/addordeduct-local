using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;

const int port = 8765;
const string url = "http://127.0.0.1:8765/";
using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("AddOrDeduct.index.html")
    ?? throw new InvalidOperationException("Embedded index.html not found.");
using var buffer = new MemoryStream();
stream.CopyTo(buffer);
byte[] page = buffer.ToArray();
string dataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AddOrDeductLocal");
Directory.CreateDirectory(dataDir);
string[] allowedKeys = ["roster", "options", "templateBytes", "records"];
var listener = new TcpListener(IPAddress.Loopback, port);
try { listener.Start(); }
catch (SocketException ex)
{
    Console.Error.WriteLine("無法啟動本機服務（連接埠 8765 可能已被占用）：" + ex.Message);
    Console.WriteLine("請關閉其他加扣分APP視窗後重試。按 Enter 結束。");
    Console.ReadLine();
    return;
}
Console.WriteLine("加扣分APP 電腦版已啟動：" + url);
Console.WriteLine("資料儲存位置：" + dataDir);
Console.WriteLine("請保留此視窗。關閉視窗即停止服務，已保存的資料不會因此刪除。");
try { Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); }
catch { Console.WriteLine("請自行用 Chrome 開啟 " + url); }
while (true)
{
    using var client = await listener.AcceptTcpClientAsync();
    try
    {
        client.ReceiveTimeout = 5000;
        client.SendTimeout = 5000;
        using var network = client.GetStream();
        using var reader = new StreamReader(network, Encoding.ASCII, false, 1024, leaveOpen: true);
        string? request = await reader.ReadLineAsync();
        if (string.IsNullOrWhiteSpace(request)) continue;
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        while (true)
        {
            string? header = await reader.ReadLineAsync();
            if (header is null || header.Length == 0) break;
            int colon = header.IndexOf(':');
            if (colon > 0) headers[header.Substring(0, colon).Trim()] = header.Substring(colon + 1).Trim();
        }
        string[] parts = request.Split(' ');
        string method = parts[0];
        string path = parts.Length > 1 ? parts[1].Split('?')[0] : "/";
        string status;
        byte[] body;
        if (path.StartsWith("/api/", StringComparison.Ordinal))
        {
            string key = path.Substring(5);
            if (Array.IndexOf(allowedKeys, key) < 0) { status = "404 Not Found"; body = []; }
            else
            {
                string filename = Path.Combine(dataDir, key + ".json");
                if (method == "GET")
                {
                    status = "200 OK";
                    body = File.Exists(filename) ? await File.ReadAllBytesAsync(filename) : Encoding.UTF8.GetBytes("null");
                }
                else if (method == "POST")
                {
                    // Reject unbounded payloads. Only the local browser may write to this loopback endpoint.
                    var lengthHeader = "";
                    // Content-Length was collected while parsing request headers.
                    if (!headers.TryGetValue("content-length", out lengthHeader) ||
                        !int.TryParse(lengthHeader, out int contentLength) || contentLength < 0 || contentLength > 25_000_000)
                    {
                        status = "413 Payload Too Large"; body = [];
                    }
                    else if (!headers.TryGetValue("origin", out var origin) || origin != "http://127.0.0.1:8765")
                    {
                        status = "403 Forbidden"; body = [];
                    }
                    else
                    {
                        var payload = new byte[contentLength];
                        await network.ReadExactlyAsync(payload);
                        using (JsonDocument.Parse(payload)) { }
                        string tmp = filename + ".tmp";
                        await File.WriteAllBytesAsync(tmp, payload);
                        File.Move(tmp, filename, true);
                        status = "200 OK"; body = Encoding.UTF8.GetBytes("{\\\"ok\\\":true}");
                    }
                }
                else if (method == "DELETE")
                {
                    if (!headers.TryGetValue("origin", out var origin) || origin != "http://127.0.0.1:8765")
                    { status = "403 Forbidden"; body = []; }
                    else { if (File.Exists(filename)) File.Delete(filename); status = "200 OK"; body = []; }
                }
                else { status = "405 Method Not Allowed"; body = []; }
            }
        }
        else if (method != "GET" && method != "HEAD") { status = "405 Method Not Allowed"; body = []; }
        else if (path is "/" or "/index.html") { status = "200 OK"; body = page; }
        else if (path == "/favicon.ico") { status = "204 No Content"; body = []; }
        else { status = "404 Not Found"; body = []; }
        byte[] head = Encoding.ASCII.GetBytes($"HTTP/1.1 {status}\r\nContent-Type: text/html; charset=utf-8\r\nContent-Length: {body.Length}\r\nCache-Control: no-store\r\nX-Content-Type-Options: nosniff\r\nConnection: close\r\n\r\n");
        await network.WriteAsync(head);
        if (method != "HEAD" && body.Length > 0) await network.WriteAsync(body);
        await network.FlushAsync();
    }
    catch (Exception ex) { Console.Error.WriteLine("請求處理失敗：" + ex.Message); }
}
