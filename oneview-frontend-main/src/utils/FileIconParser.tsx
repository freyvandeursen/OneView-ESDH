import {
  FileText,
  FileSpreadsheet,
  FileCode,
  FileImage,
  FileVideo,
  FileAudio,
  FileArchive,
  FileType,
  FileJson,
  FileTerminal,
  FileKey,
  FileQuestionMark,
} from 'lucide-react';

/**
 * Returns a file-type specific icon component based on a given type.
 *
 * Supports most common file types
 *
 * Defaults to a generic file icon if the MIME type is unrecognized.
 *
 * @param {string} type - The MIME type of the file (e.g., `"image/png"`, `"application/pdf"`).
 * @param {string} [color='black'] - Optional color string for the icon. Defaults to `'black'`.
 * @returns {React.ReactNode} A Lucide icon component corresponding to the file type.
 *
 */
export default function fileIconParser(
  type: string,
  color: string = 'black'
): React.ReactNode {
  const iconProps = { size: 30, color };

  switch (type) {
    // Text & Document Files
    case 'text/plain':
    case 'application/msword':
    case 'application/vnd.openxmlformats-officedocument.wordprocessingml.document':
      return <FileText {...iconProps} />;

    // PDF
    case 'application/pdf':
      return <FileType {...iconProps} />;

    // Excel / Spreadsheet
    case 'application/vnd.ms-excel':
    case 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet':
      return <FileSpreadsheet {...iconProps} />;

    // Images
    case 'image/png':
    case 'image/jpeg':
    case 'image/jpg':
    case 'image/gif':
    case 'image/webp':
    case 'image/svg+xml':
      return <FileImage {...iconProps} />;

    // Video
    case 'video/mp4':
    case 'video/mpeg':
    case 'video/quicktime':
      return <FileVideo {...iconProps} />;

    // Audio
    case 'audio/mpeg':
    case 'audio/wav':
    case 'audio/ogg':
      return <FileAudio {...iconProps} />;

    // Archives
    case 'application/zip':
    case 'application/x-rar-compressed':
    case 'application/x-7z-compressed':
    case 'application/x-tar':
    case '.zip':
      return <FileArchive {...iconProps} />;

    // Code / JSON / Config
    case 'application/json':
      return <FileJson {...iconProps} />;
    case 'text/javascript':
    case 'text/x-python':
    case 'text/x-java-source':
    case 'text/html':
    case 'text/css':
    case 'application/x-sh':
    case '.xlsx':
      return <FileCode {...iconProps} />;

    // Keys / Certificates
    case 'application/x-pem-file':
    case 'application/x-x509-ca-cert':
      return <FileKey {...iconProps} />;

    // Terminals / Shell
    case 'application/x-shellscript':
      return <FileTerminal {...iconProps} />;

    default:
      return <FileQuestionMark {...iconProps} />;
  }
}
