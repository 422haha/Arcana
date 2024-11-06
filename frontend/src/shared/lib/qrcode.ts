import QRCode from 'qrcode';

interface QRCodeParams {
  Margin: number;
  Background: string;
  Finder: string;
  Horizontal: string;
  Vertical: string;
  Cross: string;
  'Horizontal thickness': number;
  'Vertical thickness': number;
  'Cross thickness': number;
}

export async function createQRCode(
  url: string,
  params: QRCodeParams
): Promise<string> {
  try {
    const qrSvg = await QRCode.toString(url, {
      type: 'svg',
      color: {
        dark: params.Finder,
        light: params.Background,
      },
      margin: params.Margin,
      width: 200, // QR 코드의 크기를 조절할 수 있습니다.
    });

    // SVG를 커스터마이즈하려면 여기에서 추가 작업을 할 수 있습니다.
    // 예: 색상 변경, 스타일 추가 등

    return qrSvg;
  } catch (err) {
    console.error('QR 코드 생성 중 오류 발생:', err);
    return '';
  }
}
