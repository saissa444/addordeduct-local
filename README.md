# 加扣分APP｜Windows 本機版

本專案與原本 `addordeduct` 分開，學生資料不會上傳 GitHub。

## Windows 啟動方式（建議）
1. GitHub → Code → Download ZIP，解壓縮至固定資料夾，例如 `D:\addordeduct-local`。
2. 電腦需要安裝 Python 3（可在命令提示字元執行 `py -3 --version` 確認）。沒有 Python 可從 https://www.python.org/downloads/windows/ 安裝。
3. **雙擊 `啟動加扣分APP.bat`**，瀏覽器會開啟 `http://127.0.0.1:8765/`。
4. 第一次在這個網址使用時，重新匯入名冊及 Excel 範本。
5. 關閉 Chrome 後，之後仍用同一個啟動檔開啟，應會從 Chrome IndexedDB 還原資料。可關閉啟動後出現的命令提示字元來停止本機伺服器。
6. **不要再雙擊 `index.html`**，那是不同的儲存來源，與 `127.0.0.1` 的資料不共用。

## 隱私及備份
- 本機 HTTP 服務只綁定 `127.0.0.1`，僅供這台電腦使用，不會向網路提供學生資料。
- 名冊、範本與登錄紀錄存在 Chrome 的 IndexedDB；程式本身不將資料上傳到 GitHub。
- Excel 處理函式庫目前從 jsDelivr CDN 載入，首次使用需連網；**尚不是完全離線版本**。
- 清除 Chrome 網站資料、無痕模式、更換 Chrome 使用者設定檔、改用不同網址或電腦，都可能使本機資料無法讀取。
- IndexedDB 不是可靠的唯一備份，請妥善保存原始 Excel 檔與匯出紀錄。
- 若網址顯示連線失敗，請確認啟動檔的黑色命令提示字元視窗仍開著；若 8765 埠被占用，需先關閉占用程式。
- 在正式處理學生資料前，先用測試名冊驗證：匯入 → 關閉 Chrome → 再用啟動檔開啟 → 資料自動還原。

原版：https://saissa444.github.io/addordeduct/
