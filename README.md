# 加扣分APP｜Windows 本機版（不需安裝 Python）

這是獨立於原本線上版的 Windows 電腦版，使用 Windows 內建 PowerShell 啟動本機 HTTP 服務。

## 如何使用
1. 在 GitHub 專案按 **Code → Download ZIP**，解壓縮到固定資料夾（例如 `D:\addordeduct-local`）。
2. **雙擊 `啟動加扣分APP.bat`**，不必安裝 Python。
3. 會開啟瀏覽器網址 **http://127.0.0.1:8765/**；若沒有自動開啟，請將這個網址貼到 Chrome。
4. 第一次從這個網址使用時，匯入名冊與學校 Excel 範本。
5. 關閉 Chrome 後，再用同一個啟動檔及同一個 Chrome 設定檔開啟，應會還原本機資料。
6. 使用期間請保留黑色命令視窗；關閉該視窗或按 Ctrl+C 即停止本機服務。

**請不要直接雙擊 `index.html`**：`file://` 與 `http://127.0.0.1:8765/` 使用不同的 Chrome 資料庫，之前在 `file://` 匯入的資料不會自動搬過來。

## 資料安全及限制
- 本機服務只監聽 `127.0.0.1`，不對區域網路或網際網路開放。
- 名冊、範本與登錄紀錄由 Chrome IndexedDB 保存在本機；程式不會將它們上傳 GitHub。
- Excel 處理函式庫仍由 jsDelivr CDN 載入，所以**目前不保證完全離線**。
- Chrome 清除網站資料、使用無痕模式、切換 Chrome 設定檔或更換電腦，都可能造成無法讀取先前資料。
- IndexedDB 不是永久備份，請另外妥善保存原始 Excel 與匯出紀錄。
- 部分學校管理的 Windows 電腦可能限制 PowerShell 腳本執行；遇到限制請洽資訊管理人員，不要自行繞過機關資安政策。
- 本版本尚未在使用者電腦完成實機測試。正式使用學生資料前，先用測試名冊驗證「匯入 → 關閉 Chrome → 重新開啟 → 自動還原」。

原本線上版仍在 https://saissa444.github.io/addordeduct/ ，不受影響。


## Windows EXE：資料持久保存（新版）
Windows EXE 版本改為透過本機 API，將名冊、加扣分範本及登錄紀錄寫入目前 Windows 使用者的：
`%LOCALAPPDATA%\AddOrDeductLocal\`
（`roster.json`、`options.json`、`templateBytes.json`、`records.json`）。

請從 GitHub Actions 的 **Build portable Windows EXE** 最新成功執行紀錄下載 Artifacts，**舊版 EXE 不會自動更新**。第一次使用新版仍需重新匯入兩份 Excel；舊版 Chrome IndexedDB 資料不會自動轉移。關閉 APP 後重新開啟，應會從 Windows 資料夾還原。請先用測試資料驗證。不要刪除上述資料夾；電腦重設、切換 Windows 使用者或學校政策清除 AppData 仍可能刪除資料。請定期匯出 Excel 並另行備份，依校方個資規範管理。
