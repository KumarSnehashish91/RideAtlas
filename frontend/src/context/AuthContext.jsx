import React, { createContext, useContext, useEffect, useState } from "react";
import client from "../lib/api";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null); // null = checking, false = anon, obj = authed
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const bootstrap = async () => {
      try {
        const { data } = await client.get("/auth/me");
        setUser(data);
      } catch {
        setUser(false);
      } finally {
        setLoading(false);
      }
    };
    bootstrap();
  }, []);

  const login = async (email, password) => {
    const { data } = await client.post("/auth/login", { email, password });
    if (data.token) localStorage.setItem("ra_token", data.token);
    setUser({ id: data.id, email: data.email, name: data.name });
    return data;
  };

  const register = async (name, email, password) => {
    const { data } = await client.post("/auth/register", { name, email, password });
    if (data.token) localStorage.setItem("ra_token", data.token);
    setUser({ id: data.id, email: data.email, name: data.name });
    return data;
  };

  const logout = async () => {
    try { await client.post("/auth/logout"); } catch { /* ignore */ }
    localStorage.removeItem("ra_token");
    setUser(false);
  };

  return (
    <AuthContext.Provider value={{ user, loading, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => useContext(AuthContext);
