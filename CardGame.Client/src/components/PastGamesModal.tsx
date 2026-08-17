import React, { useEffect, useState } from "react";
import {
  ArrowUpDown,
  ChevronLeft,
  ChevronRight,
  FilterX,
  Inbox,
  Trophy,
  X,
} from "lucide-react";
import type { Game, PaginatedResult } from "../types/game";
import { gameApi } from "../services/api";
import { Button } from "./ui/button";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "./ui/table";
import { Badge } from "./ui/badge";
import { formatGameDate } from "../lib/utils";

interface PastGamesModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSelectGame: (game: Game) => void;
  onError: (error: string) => void;
}

type SortColumn = "date" | "score" | "playername";
type SortDirection = "asc" | "desc";

const pageSize = 6;

export const PastGamesModal: React.FC<PastGamesModalProps> = ({
  isOpen,
  onClose,
  onSelectGame,
  onError,
}) => {
  const [data, setData] = useState<PaginatedResult<Game> | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [page, setPage] = useState<number>(1);
  const [sortColumn, setSortColumn] = useState<SortColumn>("date");
  const [sortDirection, setSortDirection] = useState<SortDirection>("desc");
  const [filterPlayerName, setFilterPlayerName] = useState<string>("");

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

  useEffect(() => {
    if (!isOpen) return;

    const loadPastGames = async () => {
      setLoading(true);
      try {
        const result = await gameApi.getPaginatedGames(
          page,
          pageSize,
          sortColumn,
          sortDirection,
          filterPlayerName || undefined,
        );
        setData(result);
      } catch (err: any) {
        onError(err.message || "Failed to load past games.");
      } finally {
        setLoading(false);
      }
    };

    void loadPastGames();
  }, [
    isOpen,
    page,
    sortColumn,
    sortDirection,
    filterPlayerName,
    onError,
  ]);

  if (!isOpen) return null;

  const toggleSort = (column: SortColumn) => {
    setPage(1);
    if (sortColumn === column) {
      setSortDirection(sortDirection === "asc" ? "desc" : "asc");
    } else {
      setSortColumn(column);
      setSortDirection("desc");
    }
  };

  const clearFilters = () => {
    setPage(1);
    setFilterPlayerName("");
  };

  const hasActiveFilters = Boolean(filterPlayerName);

  const handleBackdropClick = (e: React.MouseEvent<HTMLDivElement>) => {
    if (e.target === e.currentTarget) {
      onClose();
    }
  };

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 p-4 backdrop-blur-sm"
      onClick={handleBackdropClick}
    >
      <div className="relative flex max-h-[90vh] w-full max-w-5xl flex-col rounded-2xl border border-border bg-card shadow-2xl">
        {/* Modal Header */}
        <div className="flex items-center justify-between border-b border-border p-6 sm:px-8">
          <div>
            <h2 className="text-xl font-semibold text-foreground sm:text-2xl">
              Past Games
            </h2>
            <p className="mt-1 text-xs text-muted-foreground sm:text-sm">
              View and filter historical game results
            </p>
          </div>

          <button
            onClick={onClose}
            className="rounded-lg p-2 text-muted-foreground transition-colors hover:bg-muted hover:text-foreground"
            aria-label="Close modal"
          >
            <X className="h-5 w-5" />
          </button>
        </div>

        {/* Modal Body */}
        <div className="flex-1 overflow-y-auto p-6 sm:p-8">
          {loading && !data ? (
            <div className="rounded-2xl border border-border bg-muted px-6 py-16 text-center text-sm text-muted-foreground">
              Loading past games...
            </div>
          ) : !data || data.items.length === 0 ? (
            <div className="flex flex-col items-center justify-center rounded-2xl border border-border bg-muted/40 p-12 text-center">
              <Inbox className="mb-3 h-8 w-8 text-muted-foreground" />
              <h3 className="text-base font-semibold text-foreground">
                No Games Found
              </h3>
              <p className="mt-1 text-xs text-muted-foreground">
                {hasActiveFilters
                  ? "No past games match your active search filter."
                  : "Deal a game to record history."}
              </p>
              {hasActiveFilters && (
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  onClick={clearFilters}
                  className="mt-4 gap-1.5 text-xs"
                >
                  <FilterX className="h-3.5 w-3.5" />
                  Clear Column Filter
                </Button>
              )}
            </div>
          ) : (
            <div className="overflow-hidden rounded-2xl border border-border bg-background shadow-sm">
              <div className="overflow-x-auto">
                <Table className="min-w-full border-separate border-spacing-0">
                  <TableHeader className="bg-muted/80 backdrop-blur-sm">
                    <TableRow>
                      {/* Date & Time Header */}
                      <TableHead
                        className="px-4 py-3 align-top font-medium cursor-pointer select-none hover:bg-muted/90 transition"
                        onClick={() => toggleSort("date")}
                      >
                        <div className="flex items-center gap-1.5 text-xs font-semibold uppercase tracking-[0.15em] text-foreground">
                          Date & Time
                          <ArrowUpDown
                            className={`h-3.5 w-3.5 transition-transform ${
                              sortColumn === "date"
                                ? sortDirection === "asc"
                                  ? "text-primary"
                                  : "rotate-180 text-primary"
                                : "text-muted-foreground/50"
                            }`}
                          />
                        </div>
                      </TableHead>

                      {/* Game ID Header */}
                      <TableHead className="px-4 py-3 align-top text-xs font-semibold uppercase tracking-[0.15em] text-foreground">
                        Game ID
                      </TableHead>

                      {/* Winner(s) Header with Inline Column Filter */}
                      <TableHead className="px-4 py-3 align-top min-w-[200px]">
                        <div
                          className="flex items-center gap-1.5 text-xs font-semibold uppercase tracking-[0.15em] text-foreground cursor-pointer select-none hover:text-primary transition"
                          onClick={() => toggleSort("playername")}
                        >
                          Winner(s)
                          <ArrowUpDown
                            className={`h-3.5 w-3.5 transition-transform ${
                              sortColumn === "playername"
                                ? sortDirection === "asc"
                                  ? "text-primary"
                                  : "rotate-180 text-primary"
                                : "text-muted-foreground/50"
                            }`}
                          />
                        </div>
                        <div className="mt-2" onClick={(e) => e.stopPropagation()}>
                          <input
                            type="text"
                            placeholder="Filter winner..."
                            value={filterPlayerName}
                            onChange={(e) => {
                              setPage(1);
                              setFilterPlayerName(e.target.value);
                            }}
                            className="w-full rounded-lg border border-input bg-card px-2.5 py-1 text-xs text-foreground placeholder:text-muted-foreground/70 outline-none transition focus:border-primary focus:ring-1 focus:ring-primary"
                          />
                        </div>
                      </TableHead>

                      {/* Top Score Header */}
                      <TableHead
                        className="px-4 py-3 align-top font-medium cursor-pointer select-none hover:bg-muted/90 transition"
                        onClick={() => toggleSort("score")}
                      >
                        <div className="flex items-center gap-1.5 text-xs font-semibold uppercase tracking-[0.15em] text-foreground">
                          Top Score
                          <ArrowUpDown
                            className={`h-3.5 w-3.5 transition-transform ${
                              sortColumn === "score"
                                ? sortDirection === "asc"
                                  ? "text-primary"
                                  : "rotate-180 text-primary"
                                : "text-muted-foreground/50"
                            }`}
                          />
                        </div>
                      </TableHead>


                      {/* Action Header with Clear Filters Button */}
                      <TableHead className="px-4 py-3 align-top text-right text-xs font-semibold uppercase tracking-[0.15em] text-foreground">
                        <div className="flex items-center justify-end gap-2">
                          <span>Action</span>
                          {hasActiveFilters && (
                            <button
                              type="button"
                              onClick={clearFilters}
                              title="Clear active column filters"
                              className="inline-flex items-center gap-1 rounded-md bg-destructive/10 px-1.5 py-0.5 text-[10px] font-semibold text-destructive hover:bg-destructive/20 transition"
                            >
                              <FilterX className="h-3 w-3" />
                              Reset
                            </button>
                          )}
                        </div>
                      </TableHead>
                    </TableRow>
                  </TableHeader>

                  <TableBody>
                    {data.items.map((game) => {
                      const winners = game.players.filter(
                        (p) => p.score?.isWinner,
                      );
                      const topScore = Math.max(
                        ...game.players.map((p) => p.score?.handSum ?? 0),
                      );

                      return (
                        <TableRow
                          key={game.id}
                          className="hover:bg-muted/40 transition-colors"
                        >
                          <TableCell className="px-4 py-3 text-xs text-foreground font-medium">
                            {formatGameDate(
                              game.createdAt ?? game.createdAtUtc,
                            )}
                          </TableCell>
                          <TableCell className="px-4 py-3 font-mono text-xs text-muted-foreground">
                            {game.id.substring(0, 8)}...
                          </TableCell>
                          <TableCell className="px-4 py-3">
                            <div className="flex flex-wrap gap-1.5">
                              {winners.length > 0 ? (
                                winners.map((winner) => (
                                  <Badge
                                    key={winner.id}
                                    variant="secondary"
                                    className="text-xs px-2 py-0.5 gap-1"
                                  >
                                    <Trophy className="h-3 w-3 text-primary" />
                                    {winner.name}
                                  </Badge>
                                ))
                              ) : (
                                <span className="text-xs text-muted-foreground">
                                  —
                                </span>
                              )}
                            </div>
                          </TableCell>
                          <TableCell className="px-4 py-3 text-xs font-bold text-foreground">
                            {topScore} pts
                          </TableCell>
                          <TableCell className="px-4 py-3 text-right">
                            <Button
                              type="button"
                              size="sm"
                              variant="outline"
                              onClick={() => onSelectGame(game)}
                              className="h-8 px-3 text-xs"
                            >
                              Inspect Game
                            </Button>
                          </TableCell>
                        </TableRow>
                      );
                    })}
                  </TableBody>
                </Table>
              </div>

              {/* Modal Pagination Footer */}
              {data.totalPages > 1 && (
                <div className="flex items-center justify-between border-t border-border bg-muted/30 px-4 py-3">
                  <Button
                    type="button"
                    variant="outline"
                    size="sm"
                    disabled={page <= 1 || loading}
                    onClick={() => setPage((prev) => Math.max(1, prev - 1))}
                    className="h-8 gap-1.5 text-xs"
                  >
                    <ChevronLeft className="h-3.5 w-3.5" />
                    Prev
                  </Button>

                  <span className="text-xs font-medium text-muted-foreground">
                    Page {data.page} of {data.totalPages} ({data.totalCount}{" "}
                    total games)
                  </span>

                  <Button
                    type="button"
                    variant="outline"
                    size="sm"
                    disabled={page >= data.totalPages || loading}
                    onClick={() =>
                      setPage((prev) => Math.min(data.totalPages, prev + 1))
                    }
                    className="h-8 gap-1.5 text-xs"
                  >
                    Next
                    <ChevronRight className="h-3.5 w-3.5" />
                  </Button>
                </div>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};
