var __fs_initialized;
var __fs_errored;
var __fs_timed_out;
var __fs_waiting_callbacks = [];

const desktop_folder_path = "/desktop/";

// 로컬 환경과 배포 환경을 구분
const isProduction = window.location.hostname !== 'localhost';
const web_server_root_for_browserfs = isProduction
  ? location.origin + '/' // 배포 환경: 도메인 루트 사용
  : '/98/'; // 로컬 환경: /98/ 경로 사용

// BrowserFS 설정
BrowserFS.configure(
  {
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
  },
  function (error) {
    if (error) {
      __fs_errored = true;
      if (__fs_waiting_callbacks.length) {
        alert("The filesystem is not available. It failed to initialize.");
      }
      __fs_waiting_callbacks = [];
      throw error;
    }
    __fs_initialized = true;
    for (var i = 0; i < __fs_waiting_callbacks.length; i++) {
      __fs_waiting_callbacks[i]();
    }
    __fs_waiting_callbacks = [];
  }
);

// 파일 시스템 초기화 시간 초과 처리
setTimeout(function () {
  __fs_timed_out = true;
  if (__fs_waiting_callbacks.length) {
    alert("The filesystem is not working.");
  }
  __fs_waiting_callbacks = [];
}, 5000);

// 파일 시스템 사용을 위한 콜백 함수
function withFilesystem(callback) {
  if (__fs_initialized) {
    callback();
  } else if (__fs_errored) {
    alert("The filesystem is not available. It failed to initialize.");
  } else if (__fs_timed_out) {
    alert("The filesystem is not working.");
  } else {
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
