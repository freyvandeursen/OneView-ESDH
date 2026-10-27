import { Pencil } from 'lucide-react';
import { Button } from '@/components/ui/button';
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from '@/components/ui/tooltip';

export function HoverEditButton() {
  return (
    <div className="relative group">
      {/* Your content */}
      <div className="p-4 border rounded-lg">Hover me to show edit button</div>

      {/* Pencil button */}
      <Tooltip>
        <TooltipTrigger asChild>
          <Button
            variant="ghost"
            size="icon"
            className="
              absolute top-2 right-2 
              h-7 w-7 
              opacity-0 group-hover:opacity-100 
              transition-opacity
            "
          >
            <Pencil className="h-4 w-4" />
          </Button>
        </TooltipTrigger>
        <TooltipContent>Edit</TooltipContent>
      </Tooltip>
    </div>
  );
}
