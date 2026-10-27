import React from 'react';
import type { S3File } from '@/models/s3file';
import FileDisplay from './fileDisplayer';

// (example future types)
// import FreshdeskDisplay from "./FreshdeskDisplay";
// import KeepersecurityDisplay from "./KeepersecurityDisplay";

type ContextDisplayRegistry = {
  [key: string]: React.FC<{ data: any }>;
};

export const contextDisplays: ContextDisplayRegistry = {
  File: ({ data }) => <FileDisplay file={data as S3File} />,
  // freshdesk: ({ data }) => <FreshdeskDisplay freshdesk={data} />,
  // keepersecurity: ({ data }) => <KeepersecurityDisplay keepersecurity={data} />,
};
