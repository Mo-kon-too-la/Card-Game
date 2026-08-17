import React, { useEffect } from "react";
import { X } from "lucide-react";
import { Button } from "./ui/button";

interface PlayerInputModalProps {
  isOpen: boolean;
  playerNames: string[];
  loading: boolean;
  onPlayerNameChange: (index: number, value: string) => void;
  onDealGame: () => void;
  onClose: () => void;
}

const playerNamePlaceholder = (index: number) => `Player ${index + 1}`;

export const PlayerInputModal: React.FC<PlayerInputModalProps> = ({
  isOpen,
  playerNames,
  loading,
  onPlayerNameChange,
  onDealGame,
  onClose,
}) => {
  useEffect(() => {
    const handleEsc = (e: KeyboardEvent) => {
      if (e.key === "Escape" && isOpen) {
        onClose();
      }
    };

    if (isOpen) {
      document.addEventListener("keydown", handleEsc);
      return () => document.removeEventListener("keydown", handleEsc);
    }
  }, [isOpen, onClose]);

  if (!isOpen) return null;

  const handleBackdropClick = (e: React.MouseEvent<HTMLDivElement>) => {
    if (e.target === e.currentTarget) {
      onClose();
    }
  };

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4 backdrop-blur-sm"
      onClick={handleBackdropClick}
    >
      <div className="w-full max-w-2xl rounded-2xl border border-border bg-card p-6 shadow-lg sm:p-8">
        <div className="mb-6 flex items-start justify-between">
          <div>
            <h2 className="text-xl font-semibold tracking-tight text-foreground sm:text-2xl">
              New Game Setup
            </h2>
            <p className="mt-1 text-sm text-muted-foreground">
              Enter names for 6 players (optional)
            </p>
          </div>
          <button
            onClick={onClose}
            className="rounded-lg p-1.5 transition-colors hover:bg-muted"
            aria-label="Close modal"
          >
            <X className="h-5 w-5 text-muted-foreground" />
          </button>
        </div>

        <div className="grid gap-3 sm:grid-cols-2">
          {playerNames.map((name, index) => (
            <label
              key={`player-${index}`}
              className="rounded-xl border border-border bg-background p-3.5 text-left"
            >
              <span className="mb-1.5 block text-xs font-medium text-muted-foreground">
                Player {index + 1}
              </span>
              <input
                type="text"
                value={name}
                onChange={(event) =>
                  onPlayerNameChange(index, event.target.value)
                }
                placeholder={playerNamePlaceholder(index)}
                className="w-full rounded-lg border border-input bg-card px-3 py-2 text-sm text-foreground placeholder:text-muted-foreground outline-none transition focus:border-ring focus:ring-2 focus:ring-ring/20"
              />
            </label>
          ))}
        </div>

        <Button
          type="button"
          className="mt-6 w-full gap-2 py-2.5 text-sm"
          onClick={onDealGame}
          disabled={loading}
        >
          {loading ? "Dealing..." : "Deal Cards"}
        </Button>
      </div>
    </div>
  );
};
