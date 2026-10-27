import type { AnyContext } from '@/models/context';

// Returns a string suitable for sorting/filtering for any context type
export function getContextSearchableValue(context: AnyContext): string {
  switch (context.type) {
    case 'file':
      return (context.data as any).name ?? '';
    // case 'freshdesk':
    //   return (context.data as any).title ?? '';
    // case 'keepersecurity':
    //   return (context.data as any).name ?? '';
    default:
      return '';
  }
}
