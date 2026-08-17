import React from "react";
import type { Card as CardType } from "../types/game";

interface PlayingCardProps {
  card: CardType;
  index: number;
}

export const PlayingCard: React.FC<PlayingCardProps> = ({ card, index }) => {
  const isRed = card.suit === "♦" || card.suit === "♥";

  return (
    <div
      className={[
        "relative flex h-[110px] w-[72px] flex-col justify-between rounded-xl border border-border bg-card p-2 shadow-sm",
        isRed ? "text-red-500" : "text-foreground",
      ].join(" ")}
      style={{ animationDelay: `${index * 80}ms` }}
    >
      <div className="flex flex-col items-start leading-none">
        <span className="text-[11px] font-bold">{card.rank}</span>
        <span className="text-base">{card.suit}</span>
      </div>

      <div className="self-center text-2xl leading-none">{card.suit}</div>

      <div className="flex rotate-180 flex-col items-start leading-none">
        <span className="text-[11px] font-bold">{card.rank}</span>
        <span className="text-base">{card.suit}</span>
      </div>

      <div
        className="absolute bottom-1 left-1 text-[9px] font-semibold text-muted-foreground"
        title={`Deck #${card.deckId}`}
      >
        D{card.deckId}
      </div>
    </div>
  );
};
