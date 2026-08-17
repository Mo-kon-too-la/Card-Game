import { useEffect, useState } from "react";
import {
  CreditCard,
  House,
  Layers3,
  Moon,
  Shuffle,
  SunMedium,
} from "lucide-react";
import {
  Navigate,
  Route,
  Routes,
  useLocation,
  useNavigate,
} from "react-router-dom";
import type { Game } from "./types/game";
import { gameApi } from "./services/api";
import { PlayerSeat } from "./components/PlayerSeat";
import { ErrorNotification } from "./components/ErrorNotification";
import { PastGamesModal } from "./components/PastGamesModal";
import { PlayerInputModal } from "./components/PlayerInputModal";
import { Button, buttonVariants } from "./components/ui/button";
import { formatGameDate } from "./lib/utils";
import "./index.css";

const defaultPlayerNames = Array.from({ length: 6 }, () => "");
const themeStorageKey = "card-game-theme";

function getSystemTheme(): boolean {
  if (typeof window === "undefined") {
    return false;
  }

  return window.matchMedia("(prefers-color-scheme: dark)").matches;
}

function getInitialTheme(): boolean {
  if (typeof window === "undefined") {
    return false;
  }

  const savedTheme = window.localStorage.getItem(themeStorageKey);
  if (savedTheme === "light" || savedTheme === "dark") {
    return savedTheme === "dark";
  }

  return getSystemTheme();
}

function ThemeToggle({
  isDark,
  onToggle,
}: {
  isDark: boolean;
  onToggle: () => void;
}) {
  return (
    <Button
      type="button"
      variant="outline"
      size="icon"
      onClick={onToggle}
      aria-label={isDark ? "Switch to light mode" : "Switch to dark mode"}
      title={isDark ? "Switch to light mode" : "Switch to dark mode"}
      className="h-10 w-10 rounded-lg"
    >
      {isDark ? (
        <SunMedium className="h-4 w-4" />
      ) : (
        <Moon className="h-4 w-4" />
      )}
    </Button>
  );
}


function HomePage({
  isDark,
  onToggleTheme,
}: {
  isDark: boolean;
  onToggleTheme: () => void;
}) {
  const location = useLocation();
  const navigate = useNavigate();
  const [currentGame, setCurrentGame] = useState<Game | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [playerNames, setPlayerNames] = useState<string[]>(defaultPlayerNames);
  const [showPlayerInputModal, setShowPlayerInputModal] =
    useState<boolean>(false);
  const [showPastGamesModal, setShowPastGamesModal] = useState<boolean>(false);

  useEffect(() => {
    const selectedGame = (location.state as { selectedGame?: Game } | null)
      ?.selectedGame;

    if (!selectedGame) {
      setCurrentGame(null);
      return;
    }

    setCurrentGame(selectedGame);
  }, [location.state]);

  const handleGoHome = () => {
    setCurrentGame(null);
    navigate("/", { replace: true, state: undefined });
  };

  const handlePlayerNameChange = (index: number, value: string) => {
    setPlayerNames((current) => {
      const nextNames = [...current];
      nextNames[index] = value;
      return nextNames;
    });
  };

  const handleDealNewGame = async () => {
    setLoading(true);
    setError(null);
    try {
      const newGame = await gameApi.createGame(playerNames);
      setCurrentGame(newGame);
      setShowPlayerInputModal(false);
    } catch (err: any) {
      setError(err.message || "Failed to deal a new game.");
    } finally {
      setLoading(false);
    }
  };

  const handleRedealCurrentGame = async () => {
    if (!currentGame) return;
    setLoading(true);
    setError(null);
    try {
      const updatedGame = await gameApi.redealGame(currentGame.id);
      setCurrentGame(updatedGame);
    } catch (err: any) {
      setError(err.message || "Failed to re-deal current game.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-background text-foreground">
      <header className="sticky top-0 z-20 border-b border-border bg-background/95 backdrop-blur-xl">
        <div className="mx-auto flex max-w-7xl items-center justify-between gap-4 px-4 py-4 sm:px-6 lg:px-8">
          <div className="flex items-center gap-3">
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={handleGoHome}
              className="gap-2"
            >
              <House className="h-4 w-4" />
              Home
            </Button>
            <div className="flex items-center gap-3">
              <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-primary text-primary-foreground font-bold text-base">
                ♠
              </div>
              <div>
                <h1 className="text-lg font-semibold tracking-tight text-foreground sm:text-xl">
                  Card Game
                </h1>
                <p className="text-xs text-muted-foreground sm:text-sm">
                  6 Players • 2 Decks
                </p>
              </div>
            </div>
          </div>

          <div className="flex flex-wrap items-center gap-3">
            <ThemeToggle isDark={isDark} onToggle={onToggleTheme} />

            <Button
              type="button"
              onClick={() => setShowPlayerInputModal(true)}
              disabled={loading}
              className="gap-2"
            >
              <CreditCard className="h-4 w-4" />
              {loading ? "Dealing..." : "Deal New Game"}
            </Button>

            <Button
              type="button"
              variant="secondary"
              onClick={handleRedealCurrentGame}
              disabled={!currentGame || loading}
              title={
                !currentGame
                  ? "Deal a game first to re-deal"
                  : "Re-deal current game"
              }
              className="gap-2"
            >
              <Shuffle className="h-4 w-4" />
              Re-deal
            </Button>

            <Button
              type="button"
              variant="outline"
              onClick={() => setShowPastGamesModal(true)}
              className="gap-2"
            >
              <Layers3 className="h-4 w-4" />
              Past Games
            </Button>
          </div>
        </div>
      </header>

      <main className="mx-auto w-full max-w-7xl px-4 py-8 sm:px-6 lg:px-8">
        <ErrorNotification message={error} onDismiss={() => setError(null)} />

        {!currentGame ? (
          <div className="flex min-h-[50vh] flex-col items-center justify-center rounded-2xl border border-border bg-card p-8 text-center shadow-sm sm:p-12">
            <div className="mb-4 flex h-14 w-14 items-center justify-center rounded-full bg-muted text-foreground">
              <CreditCard className="h-7 w-7 text-primary" />
            </div>
            <h2 className="text-2xl font-semibold tracking-tight text-foreground sm:text-3xl">
              Start a new game
            </h2>
            <p className="mt-2 text-sm text-muted-foreground sm:text-base max-w-md">
              Start a game with 6 players. Cards will be dealt from two 52-card decks.
            </p>

            <Button
              type="button"
              className="mt-6 gap-2 px-5 py-2.5 text-sm"
              onClick={() => setShowPlayerInputModal(true)}
            >
              <CreditCard className="h-4 w-4" />
              Deal New Game
            </Button>
          </div>
        ) : (
          <div className="space-y-6">
            <div className="flex flex-col gap-4 rounded-2xl border border-border bg-card p-4 shadow-sm sm:flex-row sm:items-center sm:justify-between">
              <div className="flex items-center gap-3">
                <span className="rounded-lg border border-border bg-muted px-2.5 py-1 text-xs font-semibold uppercase tracking-[0.2em] text-foreground">
                  Game #{currentGame.id.substring(0, 8)}
                </span>
                <span className="text-sm text-muted-foreground">
                  {formatGameDate(
                    currentGame.createdAt ?? currentGame.createdAtUtc,
                  )}
                </span>
              </div>

              <div className="flex flex-wrap items-center gap-4 text-xs text-muted-foreground">
                <span>Hand Score: 2-10 face, J=11, Q=12, K=13, A=11</span>
                <span>Tie-Break: ♦=1, ♥=2, ♠=3, ♣=4</span>
              </div>
            </div>

            <div className="grid gap-5 md:grid-cols-2 xl:grid-cols-3">
              {currentGame.players
                .sort((a, b) => a.seatNumber - b.seatNumber)
                .map((player) => (
                  <PlayerSeat key={player.id} player={player} />
                ))}
            </div>
          </div>
        )}

        <PlayerInputModal
          isOpen={showPlayerInputModal}
          playerNames={playerNames}
          loading={loading}
          onPlayerNameChange={handlePlayerNameChange}
          onDealGame={handleDealNewGame}
          onClose={() => setShowPlayerInputModal(false)}
        />

        <PastGamesModal
          isOpen={showPastGamesModal}
          onClose={() => setShowPastGamesModal(false)}
          onSelectGame={(selectedGame) => {
            setCurrentGame(selectedGame);
            setShowPastGamesModal(false);
          }}
          onError={(errMsg) => setError(errMsg)}
        />
      </main>

      <footer className="border-t border-border bg-background px-4 py-6 text-center text-xs text-muted-foreground sm:text-sm">
        <a href="api/swagger" className={buttonVariants({ variant: "link" })}>
          API Documentation
        </a>
      </footer>
    </div>
  );
}

export function App() {
  const [isDark, setIsDark] = useState<boolean>(getInitialTheme);

  useEffect(() => {
    document.documentElement.classList.toggle("dark", isDark);
    document.documentElement.style.colorScheme = isDark ? "dark" : "light";
    window.localStorage.setItem(themeStorageKey, isDark ? "dark" : "light");
  }, [isDark]);

  useEffect(() => {
    const mediaQuery = window.matchMedia("(prefers-color-scheme: dark)");

    const handleSystemThemeChange = () => {
      const storedTheme = window.localStorage.getItem(themeStorageKey);
      if (storedTheme === null) {
        setIsDark(mediaQuery.matches);
      }
    };

    if (typeof mediaQuery.addEventListener === "function") {
      mediaQuery.addEventListener("change", handleSystemThemeChange);
      return () =>
        mediaQuery.removeEventListener("change", handleSystemThemeChange);
    }

    mediaQuery.addListener(handleSystemThemeChange);
    return () => mediaQuery.removeListener(handleSystemThemeChange);
  }, []);

  return (
    <Routes>
      <Route
        path="/"
        element={
          <HomePage
            isDark={isDark}
            onToggleTheme={() => setIsDark((prev) => !prev)}
          />
        }
      />
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}

export default App;

