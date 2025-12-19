using QRCoder;
using System.Drawing;
using System.IO;

namespace CarRentalApp.Services;

public class QrCodeService
{
    public byte[] GenerateQrCode(string content)
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
        PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
        return qrCode.GetGraphic(20);
    }
}
