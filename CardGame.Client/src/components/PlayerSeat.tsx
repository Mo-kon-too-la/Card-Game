import React from "react";
import { Trophy } from "lucide-react";
import type { Player } from "../types/game";
import { PlayingCard } from "./PlayingCard";

interface PlayerSeatProps {
  player: Player;
}

export const PlayerSeat: React.FC<PlayerSeatProps> = ({ player }) => {
  const isWinner = player.score?.isWinner;
  const isTied = player.score?.isTiedForHighestHand;

  return (
    <div
      className={[
        "rounded-2xl border border-border bg-card p-4 shadow-sm transition hover:-translate-y-1 hover:border-ring",
        isWinner ? "border-primary/40 bg-primary/5" : "",
      ].join(" ")}
    >
      <div className="mb-4 flex items-center justify-between gap-3">
        <div className="flex items-center gap-2">
          <span className="rounded-md bg-muted px-2 py-1 text-[10px] font-semibold uppercase tracking-wider text-muted-foreground">
            Seat {player.seatNumber}
          </span>
          <h3 className="text-lg font-semibold text-foreground">
            {player.name}
          </h3>
        </div>

        {isWinner && (
          <div className="flex items-center gap-1 rounded-full bg-primary px-2.5 py-1 text-xs font-semibold text-primary-foreground">
            <Trophy className="h-3.5 w-3.5" />
            Winner
          </div>
        )}
      </div>

      <div className="mb-4 flex flex-wrap items-center justify-center gap-3">
        {player.cards.map((card, idx) => (
          <PlayingCard key={card.id || idx} card={card} index={idx} />
        ))}
      </div>

      <div className="grid gap-2 md:grid-cols-2">
        <div className="flex items-center justify-between rounded-lg bg-muted px-3 py-2 text-sm text-foreground">
          <span>Hand Score</span>
          <span className="font-bold text-foreground">
            {player.score?.handSum ?? 0}
          </span>
        </div>

        {isTied && (
          <div className="flex items-center justify-between rounded-lg border border-border bg-accent px-3 py-2 text-sm text-foreground">
            <span>Suit Score</span>
            <span className="font-bold">{player.score?.suitProduct ?? 0}</span>
          </div>
        )}
      </div>
    </div>
  );
};
