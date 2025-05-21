const { app, BrowserWindow, dialog } = require('electron');
const path = require('path');
const { spawn } = require('child_process');
const { autoUpdater } = require('electron-updater');

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

  if (app.isPackaged) {
    checkForUpdates();
  }

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

function checkForUpdates() {
  autoUpdater.autoDownload = false;

  autoUpdater.checkForUpdates();

  autoUpdater.on('update-available', (info) => {
    const releaseNotes = info.releaseNotes;
    const releaseName = info.releaseName;
    const version = info.version;

    let notes = typeof releaseNotes === 'string' ? releaseNotes : releaseNotes.map(r => r.note).join('\n');

    const message = `Uma nova versão (${version}) está disponível.\n\nNotas da versão:\n${notes}\n\nDeseja atualizar agora?`;

    dialog.showMessageBox({
      type: 'info',
      buttons: ['Atualizar', 'Agora não'],
      defaultId: 0,
      cancelId: 1,
      title: releaseName,
      message: message,
    }).then(result => {
      if (result.response === 0) {
        autoUpdater.downloadUpdate();
      }
    });
  });

  autoUpdater.on('update-downloaded', () => {
    dialog.showMessageBox({
      type: 'question',
      buttons: ['Reiniciar agora', 'Depois'],
      defaultId: 0,
      cancelId: 1,
      title: 'Atualização pronta',
      message: 'A atualização foi baixada. Deseja reiniciar agora para aplicar as mudanças?'
    }).then(result => {
      if (result.response === 0) {
        autoUpdater.quitAndInstall();
      }
    });
  });

  autoUpdater.on('error', (error) => {
    console.error('Erro ao atualizar:', error);
  });
}
