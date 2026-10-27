import React from 'react';
import styled from 'styled-components';
import { Badge } from './ui/badge';
import { Button } from './ui/button';
import fileIconParser from '@/utils/FileIconParser';
import type { S3File } from '@/models/s3file';

type FileDisplayProps = {
  file: S3File;
};

const FileDisplay: React.FC<FileDisplayProps> = ({ file }) => (
  <FileButton variant="outline">
    <FileLeft>
      {fileIconParser(file.type ?? '', 'orange')}
      {/* <FileName>{file.name}</FileName> */}
    </FileLeft>
    <FileBadge variant="outline">{file.type}</FileBadge>
  </FileButton>
);

export default FileDisplay;

const FileButton = styled(Button)`
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.5rem 1rem;
  cursor: pointer;
  overflow: hidden;
`;

const FileLeft = styled.div`
  display: flex;
  align-items: center;
  gap: 0.75rem;
  min-width: 0;
`;

// const FileName = styled.p`
//   margin: 0;
//   font-weight: 500;
//   white-space: nowrap;
//   overflow: hidden;
//   text-overflow: ellipsis;
// `;

const FileBadge = styled(Badge)`
  font-size: 0.65rem;
  // not sure if orange is cool?
  // border: 1px solid orange;
  font-weight: 500;
`;
