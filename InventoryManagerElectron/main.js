const { app, BrowserWindow } = require('electron');
const path = require('path');
const { spawn } = require('child_process');

let apiProcess;

function createWindow() {
  const win = new BrowserWindow({
    width: 1200,
    height: 800,
    webPreferences: {
      contextIsolation: true
    }
  });

  if (!app.isPackaged) {
    // Durante o desenvolvimento
    win.loadFile('../InventoryManagerFront/dist/spa/index.html');
  } else {
    // Após empacotado
    win.loadFile(path.join(process.resourcesPath, 'frontend', 'index.html'));
  }
}

function getApiPath() {
  // Durante o desenvolvimento
  if (!app.isPackaged) {
    return path.join(__dirname, '../InventoryManagerApi/publish/InventoryManagerApi.exe');
  }
  // Após empacotado
  return path.join(process.resourcesPath, 'api', 'InventoryManagerApi.exe');
}

app.whenReady().then(() => {
  const apiPath = path.join(__dirname, '../InventoryManagerApi/publish/InventoryManagerApi.exe');
  apiProcess = spawn(getApiPath());

  apiProcess.stdout.on('data', data => console.log(`API: ${data}`));
  apiProcess.stderr.on('data', data => console.error(`API Error: ${data}`));

  createWindow();

  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) createWindow();
  });
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') app.quit();
});

app.on('will-quit', () => {
  if (apiProcess) apiProcess.kill();
});
