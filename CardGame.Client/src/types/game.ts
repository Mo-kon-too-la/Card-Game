export interface Card {
  id: string;
  playerId: string;
  rank: string; // "2"-"10", "J", "Q", "K", "A"
  suit: string; // "♦", "♥", "♠", "♣"
  value: number; // 2-10, J=11, Q=12, K=13, A=11
  suitValue: number; // ♦=1, ♥=2, ♠=3, ♣=4
  deckId: number; // 1 or 2
}

export interface Score {
  id: string;
  playerId: string;
  handSum: number;
  suitProduct: number;
  isTiedForHighestHand: boolean;
  isWinner: boolean;
}

export interface Player {
  id: string;
  gameId: string;
  seatNumber: number;
  name: string;
  cards: Card[];
  score: Score;
}

export interface Game {
  id: string;
  createdAt?: string;
  createdAtUtc?: string;
  players: Player[];
}

export interface PaginatedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}
