var __fs_initialized;
var __fs_errored;
var __fs_timed_out;
var __fs_waiting_callbacks = [];
var __welcome_shown = false;

const desktop_folder_path = "/desktop/";

// 로컬 환경과 배포 환경을 구분
const isProduction = window.location.hostname !== 'localhost';
const web_server_root_for_browserfs = isProduction
  ? location.origin + '/98/'
  : '/98/';

// 웰컴 메시지 표시 함수
const WELCOME_SHOWN_KEY = 'arcanaWelcomeShown';

// 첫 방문인지 체크하여 웰컴 메시지 표시 함수 호출
function showWelcomeMessage() {
  // iframe 내부에서는 메시지를 표시하지 않음
  if (window !== window.top) return;

  // 이미 환영 메시지를 표시한 경우 로컬 스토리지를 확인
  if (__welcome_shown || localStorage.getItem(WELCOME_SHOWN_KEY)) return;

  // 환영 메시지를 표시한 것으로 설정
  __welcome_shown = true;
  localStorage.setItem(WELCOME_SHOWN_KEY, 'true');

  try {
    if (typeof showMessageBox === 'function') {
      showMessageBox({
        title: "ARCANA",
        message: "Welcome to ARCANA!",
        iconID: "info"
      });
    }
  } catch (err) {
    console.error("Error showing welcome message:", err);
  }
}

// BrowserFS 설정
BrowserFS.configure({
  fs: "OverlayFS",
  options: {
    writable: {
      fs: "IndexedDB",
      options: { storeName: "C:" }
    },
    readable: {
      fs: "XmlHttpRequest",
      options: {
        index: web_server_root_for_browserfs + "filesystem-index.json",
        baseUrl: web_server_root_for_browserfs
      }
    }
  }
}, function(error) {
  if (error) {
    console.error("Filesystem initialization error:", error);
    __fs_errored = true;
    if (__fs_waiting_callbacks.length) {
      console.error("The filesystem is not available. It failed to initialize.");
    }
    __fs_waiting_callbacks = [];
    return;
  }

  __fs_initialized = true;
  const fs = BrowserFS.BFSRequire('fs');

  try {
    if (!fs.existsSync(desktop_folder_path)) {
      fs.mkdirSync(desktop_folder_path);
    }
  } catch (err) {
    console.error("Error creating desktop folder:", err);
  }

  // 대기 중인 콜백 순차적으로 실행
  const executeCallbacks = () => {
    if (__fs_waiting_callbacks.length > 0) {
      const callback = __fs_waiting_callbacks.shift();
      try {
        callback();
      } catch (err) {
        console.error("Error in callback:", err);
      }
      setTimeout(executeCallbacks, 0);
    } else {
      // 모든 콜백이 실행된 후 웰컴 메시지 표시
      if (!__welcome_shown) {
        showWelcomeMessage();
      }
    }
  };

  executeCallbacks();
});

// 파일 시스템 초기화 시간 초과 처리
setTimeout(function() {
  if (!__fs_initialized && !__fs_errored) {
    __fs_timed_out = true;
    console.warn("Filesystem initialization timed out");
  }
}, 10000);

// 파일 시스템 사용을 위한 콜백 함수
function withFilesystem(callback) {
  if (__fs_initialized) {
    try {
      callback();
    } catch (err) {
      console.error("Error in filesystem callback:", err);
    }
  } else if (!__fs_errored && !__fs_timed_out) {
    __fs_waiting_callbacks.push(callback);
  }
}

// 파일 경로에서 파일 이름 추출
function file_name_from_path(file_path) {
  return file_path.split("\\").pop().split("/").pop();
}

// 파일 경로에서 확장자 추출
function file_extension_from_path(file_path) {
  return (file_path.match(/\.(\w+)$/) || [, ""])[1];
}

// 전역 변수로 내보내기
window.desktop_folder_path = desktop_folder_path;
window.withFilesystem = withFilesystem;
window.file_name_from_path = file_name_from_path;
window.file_extension_from_path = file_extension_from_path;