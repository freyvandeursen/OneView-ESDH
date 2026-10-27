import type { Label } from './label';
import type { S3File } from './s3file';

// Base Context type, generic over the content type T
export type Context<T> = {
  name: string;
  type: string; // the context type, e.g., "file", "note", "image", etc
  typeDescription: string | null;
  createdAt: Date;
  lastModified: Date;
  referenceLink: string | null;
  data: T; // the wrapped object
  labels: Label[] | null;
  customerId: string;
  contextId: number;
};

export type AnyContext = Context<S3File>;
// | Context<Note>
// | Context<Image>;
