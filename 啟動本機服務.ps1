# Windows PowerShell 5.1 compatible, loopback-only static HTTP server.
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$port = 8765
$listener = New-Object System.Net.Sockets.TcpListener([System.Net.IPAddress]::Loopback, $port)
try {
  $listener.Start()
  Write-Host 'Add/Deduct local app: http://127.0.0.1:8765/'
  Write-Host 'Keep this window open while using the app. Press Ctrl+C to stop.'
  Start-Process 'http://127.0.0.1:8765/'
  while ($true) {
    $client = $listener.AcceptTcpClient()
    try {
      $client.ReceiveTimeout = 5000
      $client.SendTimeout = 5000
      $stream = $client.GetStream()
      $reader = New-Object System.IO.StreamReader($stream, [System.Text.Encoding]::ASCII, $false, 1024, $true)
      $line = $reader.ReadLine()
      if ([string]::IsNullOrWhiteSpace($line)) { continue }
      while ($true) {
        $header = $reader.ReadLine()
        if ($null -eq $header -or $header -eq '') { break }
      }
      $parts = $line.Split(' ')
      $method = $parts[0]
      $urlPath = $parts[1].Split('?')[0]
      $status = '200 OK'
      $mime = 'text/html; charset=utf-8'
      $body = [byte[]]@()
      if ($method -ne 'GET' -and $method -ne 'HEAD') {
        $status = '405 Method Not Allowed'
      } elseif ($urlPath -eq '/' -or $urlPath -eq '/index.html') {
        $body = [System.IO.File]::ReadAllBytes((Join-Path $root 'index.html'))
      } elseif ($urlPath -eq '/favicon.ico') {
        $status = '204 No Content'
      } else {
        $status = '404 Not Found'
      }
      $head = "HTTP/1.1 $status`r`nContent-Type: $mime`r`nContent-Length: $($body.Length)`r`nCache-Control: no-store`r`nX-Content-Type-Options: nosniff`r`nConnection: close`r`n`r`n"
      $headBytes = [System.Text.Encoding]::ASCII.GetBytes($head)
      $stream.Write($headBytes, 0, $headBytes.Length)
      if ($method -ne 'HEAD' -and $body.Length -gt 0) { $stream.Write($body, 0, $body.Length) }
      $stream.Flush()
    } catch {
      Write-Warning $_.Exception.Message
    } finally {
      if ($client) { $client.Close() }
    }
  }
} catch {
  Write-Host ('Unable to start local app: ' + $_.Exception.Message) -ForegroundColor Red
  Write-Host 'Port 8765 may already be in use or PowerShell may be restricted.'
  Read-Host 'Press Enter to close'
} finally {
  $listener.Stop()
}
