import React from "react";
import { AlertTriangle, X } from "lucide-react";

import { Button } from "./ui/button";

interface ErrorNotificationProps {
  message: string | null;
  onDismiss: () => void;
}

export const ErrorNotification: React.FC<ErrorNotificationProps> = ({
  message,
  onDismiss,
}) => {
  if (!message) return null;

  return (
    <div className="mb-6 rounded-xl border border-destructive/20 bg-destructive/5 p-4 text-destructive shadow-sm">
      <div className="flex items-start gap-3">
        <div className="mt-0.5 rounded-md bg-destructive/10 p-2">
          <AlertTriangle className="h-4 w-4" />
        </div>

        <div className="flex-1">
          <p className="text-sm font-semibold uppercase tracking-[0.18em] text-destructive/90">
            API Error Encountered
          </p>
          <p className="mt-1 text-sm text-foreground">{message}</p>
        </div>

        <Button
          type="button"
          variant="ghost"
          size="icon"
          onClick={onDismiss}
          title="Dismiss error"
          aria-label="Dismiss error"
          className="h-8 w-8 text-destructive hover:bg-destructive/10"
        >
          <X className="h-4 w-4" />
        </Button>
      </div>
    </div>
  );
};
