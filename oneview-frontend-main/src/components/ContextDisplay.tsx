import type { Context } from '@/models/context';
import { contextDisplays } from './ContextDisplayRegistry';

export function ContextDisplay<T>({ context }: { context: Context<T> }) {
  const DisplayComponent = contextDisplays[context.type];

  if (!DisplayComponent) {
    return (
      <div className="text-muted-foreground italic">
        Unsupported context type: {context.type}
      </div>
    );
  }

  return <DisplayComponent data={context.data} />;
}
