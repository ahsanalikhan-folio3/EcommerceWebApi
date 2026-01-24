









"use client";
import React, { FC, useCallback, useEffect, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";
import { Check, CheckCheck } from "lucide-react";

// ============================================================================
// TYPE DEFINITIONS
// ============================================================================

interface User {
  id: string;
  email: string;
  role: "Seller" | "Customer";
}

interface Message {
  id?: number;
  content: string;
  senderId: number;
  isSender: boolean;
  isRead: boolean;
  senderRole?: string;
  messagedAt?: string;
}

interface JWTClaims {
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": string;
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress": string;
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": "Seller" | "Customer";
  exp: number;
}

// ============================================================================
// CUSTOM HOOKS
// ============================================================================

const useAuth = (token: string) => {
  const [user, setUser] = useState<User | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!token) {
      setUser(null);
      setError(null);
      return;
    }

    try {
      // Decode JWT manually (base64)
      const base64Url = token.split(".")[1];
      const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split("")
          .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
          .join("")
      );

      const claims: JWTClaims = JSON.parse(jsonPayload);

      // Check token expiration
      const currentTime = Math.floor(Date.now() / 1000);
      if (claims.exp < currentTime) {
        setError("Token has expired");
        setUser(null);
        return;
      }

      // Map claims to User object
      const mappedUser: User = {
        id: claims["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"],
        email: claims["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"],
        role: claims["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
      };

      setUser(mappedUser);
      setError(null);
    } catch (err) {
      setError("Invalid token format");
      setUser(null);
      console.error("JWT decode error:", err);
    }
  }, [token]);

  return { user, error };
};

// ============================================================================
// COMPONENTS
// ============================================================================

const MessageItem: FC<{ message: Message }> = ({ message }) => {
  return (
    <div
      className={`flex ${message.isSender ? "justify-end" : "justify-start"}`}
    >
      <div
        className={`max-w-xs px-4 py-2 rounded-lg ${
          message.isSender
            ? "bg-blue-500 text-white"
            : "bg-gray-200 text-gray-800"
        }`}
      >
        <p className="break-words">{message.content}</p>
        {message.isSender && (
          <div className="flex justify-end mt-1">
            {message.isRead ? (
              <CheckCheck className="w-4 h-4 text-blue-200" />
            ) : (
              <Check className="w-4 h-4 text-gray-300" />
            )}
          </div>
        )}
      </div>
    </div>
  );
};

const MessageList: FC<{ messages: Message[] }> = ({ messages }) => {
  const messagesEndRef = useRef<HTMLDivElement | null>(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  if (messages.length === 0) {
    return (
      <div className="flex items-center justify-center h-full text-gray-400">
        <p>No messages yet. Start chatting!</p>
      </div>
    );
  }

  return (
    <>
      {messages.map((msg, i) => (
        <MessageItem key={msg.id ?? `temp-${i}`} message={msg} />
      ))}
      <div ref={messagesEndRef} />
    </>
  );
};

const ChatWindow: FC<{
  user: User;
  token: string;
  onDisconnect: () => void;
}> = ({ user, token, onDisconnect }) => {
  const [message, setMessage] = useState("");
  const [messages, setMessages] = useState<Message[]>([]);
  const [isConnected, setIsConnected] = useState(false);

  const connectionRef = useRef<signalR.HubConnection | null>(null);

  const startConnection = useCallback(async () => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5208/hubs/chat", {
        accessTokenFactory: () => token,
        transport: signalR.HttpTransportType.WebSockets, // force WebSocket
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    connection.on("ReceiveMessage", (incomingMessage: any) => {
      console.log(incomingMessage);
      console.log("I am here");
      
      // Only add message if it's not from the current user (to avoid duplicates)
      const messageData = {
        id: incomingMessage.id,
        content: incomingMessage.content || incomingMessage,
        senderId: incomingMessage.senderId || 0,
        isSender: false,
        isRead: false
      };
      
      setMessages((prev) => {
        // Don't add if we already have this message ID
        if (messageData.id && prev.some(m => m.id === messageData.id)) {
          return prev;
        }
        return [...prev, messageData];
      });
    });

    connection.onreconnecting(() => setIsConnected(false));
    connection.onreconnected(() => setIsConnected(true));
    connection.onclose(() => setIsConnected(false));

    try {
      await connection.start();
      console.log("SignalR connected.");
      connectionRef.current = connection;
      setIsConnected(true);
    } catch (error) {
      console.error("SignalR connection error.", error);
      setIsConnected(false);
    }
  }, [token]);

  const fetchChatHistory = useCallback(async () => {
    if (!isConnected) return;

    try {
      const response = await fetch("http://localhost:5208/api/chat/2", {
        method: "GET",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });

      const json = await response.json();

      const fetchedMessages: Message[] = json.data.messages.map((msg: any) => ({
        id: msg.id,
        content: msg.content,
        senderId: msg.senderId,
        isRead: msg.isRead || false,
        isSender: String(msg.senderId) === user.id,
        senderRole: msg.senderRole,
        messagedAt: msg.messagedAt,
      }));

      setMessages(fetchedMessages);
    } catch (err) {
      console.error("Failed to fetch chat history", err);
    }
  }, [isConnected, token, user.id]);

  useEffect(() => {
    startConnection();
    return () => {
      connectionRef.current?.stop();
    };
  }, [startConnection]);

  useEffect(() => {
    if (isConnected) {
      fetchChatHistory();
    }
  }, [isConnected, fetchChatHistory]);

  const handleSendMessage = useCallback(async () => {
    const trimmed = message.trim();
    if (!trimmed || !isConnected) return;

    // Create optimistic message without ID (will be replaced when server confirms)
    const tempId = Date.now(); // Temporary unique ID
    const newMessage: Message = {
      id: tempId,
      content: trimmed,
      senderId: Number(user.id),
      isSender: true,
      isRead: false,
    };

    // console.log("i am here");
    

    setMessages((prev) => [...prev, newMessage]);
    setMessage("");

    try {
      const response = await fetch("http://localhost:5208/api/chat/2/message", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ content: trimmed }),
      });

      // Optionally: Update the temp message with real ID from server
      if (response.ok) {
        const data = await response.json();
        if (data.id) {
          setMessages((prev) =>
            prev.map((m) => (m.id === tempId ? { ...m, id: data.id } : m))
          );
        }
      }
    } catch (err) {
      console.error("Failed to send message", err);
      // Optionally: Remove the optimistic message on error
      setMessages((prev) => prev.filter((m) => m.id !== tempId));
    }
  }, [message, token, isConnected, user.id]);

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSendMessage();
    }
  };

  return (
    <div className="bg-white rounded-lg shadow w-full max-w-4xl h-[600px] flex flex-col">
      {/* Header */}
      <div className="border-b px-6 py-4">
        <div className="flex justify-between items-center">
          <div>
            <h1 className="text-xl font-semibold text-gray-800">
              Chat Application
            </h1>
            <p className="text-sm text-gray-500 mt-1">
              {user.email} ({user.role}) •{" "}
              <span className={isConnected ? "text-green-600" : "text-red-600"}>
                {isConnected ? "Connected" : "Disconnected"}
              </span>
            </p>
          </div>
          <button
            onClick={onDisconnect}
            className="px-4 py-2 text-sm bg-red-500 hover:bg-red-600 text-white rounded"
          >
            Disconnect
          </button>
        </div>
      </div>

      {/* Messages */}
      <div className="flex-1 overflow-y-auto p-6 space-y-3">
        <MessageList messages={messages} />
      </div>

      {/* Message Input */}
      <div className="border-t px-6 py-4">
        <div className="flex gap-3">
          <input
            className="flex-1 px-3 py-2 border border-gray-300 text-black rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
            placeholder="Type a message..."
            value={message}
            onChange={(e) => setMessage(e.target.value)}
            onKeyPress={handleKeyPress}
            disabled={!isConnected}
          />
          <button
            onClick={handleSendMessage}
            disabled={!message.trim() || !isConnected}
            className={`px-6 py-2 rounded font-medium ${
              message.trim() && isConnected
                ? "bg-blue-500 hover:bg-blue-600 text-white"
                : "bg-gray-300 text-gray-500 cursor-not-allowed"
            }`}
          >
            Send
          </button>
        </div>
      </div>
    </div>
  );
};

// ============================================================================
// MAIN APP
// ============================================================================

const Chat: FC = () => {
  const [token, setToken] = useState("");
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const { user, error } = useAuth(token);

  const handleConnect = () => {
    if (user && !error) {
      setIsAuthenticated(true);
    }
  };

  const handleDisconnect = () => {
    setIsAuthenticated(false);
    setToken("");
  };

  return (
    <div className="min-h-screen bg-gray-100 flex items-center justify-center p-4">
      {!isAuthenticated ? (
        <div className="bg-white rounded-lg shadow w-full max-w-md p-6">
          <h1 className="text-2xl font-bold text-gray-800 mb-4">
            Chat Login
          </h1>

          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Access Token
              </label>
              <input
                className="w-full px-3 py-2 border border-gray-300 rounded text-black focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="Paste your JWT token"
                value={token}
                onChange={(e) => setToken(e.target.value)}
                type="password"
              />
            </div>

            {error && (
              <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
                {error}
              </div>
            )}

            {user && !error && (
              <div className="bg-green-50 border border-green-200 text-green-700 px-4 py-3 rounded">
                <p className="font-medium">Token Valid</p>
                <p className="text-sm">
                  User: {user.email} ({user.role})
                </p>
              </div>
            )}

            <button
              onClick={handleConnect}
              disabled={!user || !!error}
              className={`w-full px-6 py-3 rounded font-medium ${
                user && !error
                  ? "bg-blue-500 hover:bg-blue-600 text-white"
                  : "bg-gray-300 text-gray-500 cursor-not-allowed"
              }`}
            >
              Connect to Chat
            </button>
          </div>
        </div>
      ) : (
        user && <ChatWindow user={user} token={token} onDisconnect={handleDisconnect} />
      )}
    </div>
  );
};

export default Chat;