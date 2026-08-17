import axios from "axios";
import type { Game, PaginatedResult } from "../types/game";

export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  errors?: Record<string, string[]>;
}

const API_BASE = "/api/games";

const api = axios.create({
  baseURL: API_BASE,
  headers: {
    "Content-Type": "application/json",
  },
});

function getErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    const responseData = error.response?.data as
      | ProblemDetails
      | string
      | undefined;

    if (typeof responseData === "string" && responseData.trim()) {
      return responseData;
    }

    if (responseData && typeof responseData === "object") {
      const detail = responseData.detail?.trim();
      if (detail) {
        return detail;
      }

      const title = responseData.title?.trim();
      if (title) {
        return title;
      }

      const messages = Object.values(responseData.errors ?? {}).flatMap(
        (values) => values.filter((value) => value && value.trim().length > 0),
      );

      if (messages.length > 0) {
        return messages.join(" ");
      }
    }

    if (error.response) {
      return `HTTP Error ${error.response.status}: ${error.response.statusText}`;
    }

    return error.message || "Request failed.";
  }

  return error instanceof Error ? error.message : "Request failed.";
}

function normalizePlayerNames(playerNames: string[]): string[] {
  return Array.from({ length: 6 }, (_, index) => {
    const name = playerNames[index]?.trim() ?? "";
    return name || `Player ${index + 1}`;
  });
}

export const gameApi = {
  async createGame(playerNames: string[]): Promise<Game> {
    const normalizedNames = normalizePlayerNames(playerNames);

    try {
      const response = await api.post("", normalizedNames);
      const locationHeader =
        response.headers.location ?? response.headers.Location;

      if (!locationHeader) {
        throw new Error("Game created but no location was returned.");
      }

      const location = new URL(locationHeader, "http://localhost");
      const gameId = location.pathname.split("/").filter(Boolean).at(-1);

      if (!gameId) {
        throw new Error(
          "Game created but the ID could not be read from the response location.",
        );
      }

      return this.getGameById(gameId);
    } catch (error) {
      throw new Error(getErrorMessage(error));
    }
  },

  async redealGame(gameId: string): Promise<Game> {
    try {
      const response = await api.post<Game>(`/${gameId}/redeal`);
      return response.data;
    } catch (error) {
      throw new Error(getErrorMessage(error));
    }
  },

  async getGameById(gameId: string): Promise<Game> {
    try {
      const response = await api.get<Game>(`/${gameId}`);
      return response.data;
    } catch (error) {
      throw new Error(getErrorMessage(error));
    }
  },

  async getPaginatedGames(
    page: number = 1,
    pageSize: number = 10,
    sortBy?: string,
    sortDirection?: string,
    filterPlayerName?: string,
  ): Promise<PaginatedResult<Game>> {
    try {
      const response = await api.get<PaginatedResult<Game>>("", {
        params: {
          page,
          pageSize,
          ...(sortBy && { sortBy }),
          ...(sortDirection && { sortDirection }),
          ...(filterPlayerName && { filterPlayerName }),
        },
      });
      return response.data;
    } catch (error) {
      throw new Error(getErrorMessage(error));
    }
  },
};
