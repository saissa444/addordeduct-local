# 加扣分APP｜電腦本機版

以原本 addordeduct 專案為基礎，獨立提供 Windows + Chrome 使用。

## 使用方式
1. 在 GitHub 專案頁按 **Code → Download ZIP**，解壓縮到固定資料夾。
2. 用 Chrome 開啟 `index.html`。首次需有網路以載入 Excel 處理函式庫。
3. 首次匯入名冊與學校 Excel 範本，並照常登錄。
4. 以**同一台電腦、同一個 Chrome 使用者設定檔、同一個資料夾中的 index.html** 再次開啟，App 會嘗試自動還原名冊、範本和登錄紀錄。
5. 需要換名冊或範本時，重新選檔即可覆寫本機保存版本。

## 資料安全
- 名冊、範本及登錄紀錄只保存在瀏覽器的 **IndexedDB**，不會寫入 GitHub，也沒有傳送學生資料的 API。
- Excel 處理函式庫目前從 jsDelivr CDN 載入；程式首次啟動需要網路。**尚非完全離線版本**。
- 不要使用無痕模式。瀏覽器清除網站資料、移動 HTML 檔案、切換 Chrome 設定檔或換電腦，都可能使資料無法還原。
- 建議另存原始 Excel 名冊及匯出紀錄作為備份；IndexedDB 不是永久備份。
- 不建議在共用電腦儲存學生資料。可使用「清除本機已儲存資料」刪除。
- 若 Chrome 對本機 `file://` 網頁限制 IndexedDB，請使用固定的本機 HTTP 服務網址（例如 `http://localhost:8000`）開啟。不同網址的本機資料庫彼此獨立。
- **尚未在使用者的 Windows/Chrome 環境完成實機驗證**，正式使用前請先以測試名冊確認重新開啟後可自動還原及匯出。

## 與原專案的差異
原專案仍是 https://saissa444.github.io/addordeduct/ ，不受本專案影響。
