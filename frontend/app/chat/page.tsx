// "use client";
// import React, { FC, useCallback, useEffect, useRef, useState } from "react";
// import * as signalR from "@microsoft/signalr";
// import { json } from "stream/consumers";

// /* =======================
//    Input Component
// ======================= */

// type InputProps = {
//   labelText?: string;
//   type?: React.HTMLInputTypeAttribute;
//   placeholder: string;
//   value: string;
//   onChange: (value: string) => void;
// };

// const Input: FC<InputProps> = ({
//   labelText,
//   type = "text",
//   placeholder,
//   value,
//   onChange,
// }) => {
//   const id = labelText?.replace(/\s+/g, "-").toLowerCase();

//   return (
//     <div className="flex flex-col gap-1 w-full">
//       {labelText && (
//         <label htmlFor={id} className="text-sm font-medium">
//           {labelText}
//         </label>
//       )}
//       <input
//         id={id}
//         className="p-2 border rounded w-full outline-none focus:ring-2 focus:ring-blue-400"
//         type={type}
//         placeholder={placeholder}
//         value={value}
//         onChange={(e) => onChange(e.target.value)}
//       />
//     </div>
//   );
// };

// /* =======================
//    Send Button
// ======================= */

// type SendButtonProps = {
//   onClick: () => void;
//   disabled?: boolean;
// };

// const SendButton: FC<SendButtonProps> = ({ onClick, disabled }) => {
//   return (
//     <button
//       type="button"
//       disabled={disabled}
//       onClick={onClick}
//       className={`p-2 rounded text-white transition
//         ${disabled ? "bg-blue-300 cursor-not-allowed" : "bg-blue-500 hover:bg-blue-600"}
//         sm:w-[25%] w-full`}
//     >
//       Send Message
//     </button>
//   );
// };

// /* =======================
//    Message Bubble
// ======================= */

// type MessageProps = {
//   message: string;
//   isSender: boolean;
// };

// const Message: FC<MessageProps> = ({ message, isSender }) => {
//   return (
//     <div
//       className={`max-w-[70%] p-2 rounded text-white
//         ${isSender ? "self-end bg-blue-500" : "self-start bg-gray-400"}`}
//     >
//       {message}
//     </div>
//   );
// };

// /* =======================
//    Chat Component
// ======================= */

// const Chat: FC = () => {
//   const [token, setToken] = useState("");
//   const [message, setMessage] = useState("");
//   const [messages, setMessages] = useState<
//     { content: string; isSender: boolean }[]
//   >([]);

//   const connectionRef = useRef<signalR.HubConnection | null>(null);

//   const startConnection = useCallback(async () => {
//     if (!token) return;
//     /* Start Connection */
//     const connection = new signalR.HubConnectionBuilder()
//       .withUrl("http://localhost:5208/hubs/chat", {
//         accessTokenFactory: () => token,
//       })
//       .withAutomaticReconnect()
//       .configureLogging(signalR.LogLevel.Information)
//       .build();

//     connection.on("ReceiveMessage", (message: string) => {
//       setMessages((prev) => [...prev, { content: message, isSender: false }]);
//     });

//     try {
//       await connection.start();
//       console.log("SignalR connected.");
//       connectionRef.current = connection;
//     } catch (error) {
//       console.error("SignalR connection error.", error);
//     }
//   }, [token]);

//   (useEffect(() => {
//     return () => {
//       connectionRef.current?.stop();
//     };
//   }),
//     []);

//   const handleSendMessage = useCallback(async () => {
//     const trimmed = message.trim();
//     if (!trimmed) return;

//     // Optimistic UI
//     setMessages((prev) => [...prev, { content: trimmed, isSender: true }]);
//     setMessage("");

//     try {
//       await fetch("http://localhost:5208/chat/2/message", {
//         method: "POST",
//         headers: {
//           "Content-Type": "application/json",
//           Authorization: `Bearer ${token}`,
//         },
//         body: JSON.stringify({ content: trimmed }),
//       });
//     } catch (err) {
//       console.error("Failed to send message", err);
//     }
//   }, [message, token]);

//   return (
//     <main className="rounded flex flex-col gap-6 p-5 w-full mx-auto">
//       <section>
//         {/* Token */}
//         <input
//           className="p-2 border rounded"
//           placeholder="Enter access token..."
//           value={token}
//           onChange={(e) => setToken(e.target.value)}
//         />

//         <button
//           onClick={startConnection}
//           className="bg-green-500 text-white p-2 rounded"
//         >
//           Connect
//         </button>
//       </section>

//       {/* Messages */}
//       <section className="flex flex-col gap-3 min-h-50">
//         {messages.map((msg, i) => (
//           <div
//             key={i}
//             className={`max-w-[70%] p-2 rounded text-white
//               ${msg.isSender ? "self-end bg-blue-500" : "self-start bg-gray-400"}`}
//           >
//             {msg.content}
//           </div>
//         ))}
//       </section>

//       {/* Message Input */}
//       <section className="flex gap-2">
//         <input
//           className="p-2 border rounded w-full"
//           placeholder="Enter message..."
//           value={message}
//           onChange={(e) => setMessage(e.target.value)}
//         />
//         <button
//           onClick={handleSendMessage}
//           disabled={!message.trim()}
//           className="bg-blue-500 text-white p-2 rounded"
//         >
//           Send
//         </button>
//       </section>
//     </main>
//   );
// };

// export default Chat;

"use client";
import React, { FC, useCallback, useEffect, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";

const Chat: FC = () => {
  const [token, setToken] = useState("");
  const [message, setMessage] = useState("");
  const [messages, setMessages] = useState<
    { content: string; isSender: boolean }[]
  >([]);
  const [isConnected, setIsConnected] = useState(false);

  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const messagesEndRef = useRef<HTMLDivElement | null>(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  };

  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  const startConnection = useCallback(async () => {
    if (!token) return;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5208/hubs/chat", {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    connection.on("ReceiveMessage", (message: string) => {
      setMessages((prev) => [...prev, { content: message, isSender: false }]);
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

  useEffect(() => {
    return () => {
      connectionRef.current?.stop();
    };
  }, []);

  const handleSendMessage = useCallback(async () => {
    const trimmed = message.trim();
    if (!trimmed || !isConnected) return;

    setMessages((prev) => [...prev, { content: trimmed, isSender: true }]);
    setMessage("");

    try {
      await fetch("http://localhost:5208/chat/2/message", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify({ content: trimmed }),
      });
    } catch (err) {
      console.error("Failed to send message", err);
    }
  }, [message, token, isConnected]);

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSendMessage();
    }
  };

  return (
    <div className="min-h-screen bg-gray-100 flex items-center justify-center p-4">
      <div className="bg-white rounded-lg shadow w-full max-w-4xl h-[600px] flex flex-col">
        {/* Header */}
        <div className="border-b px-6 py-4">
          <h1 className="text-xl font-semibold text-gray-800">
            Chat Application
          </h1>
          <p className="text-sm text-gray-500 mt-1">
            Status:{" "}
            <span className={isConnected ? "text-green-600" : "text-red-600"}>
              {isConnected ? "Connected" : "Disconnected"}
            </span>
          </p>
        </div>

        {/* Connection Section */}
        {!isConnected && (
          <div className="border-b px-6 py-4 bg-gray-50">
            <div className="flex gap-3">
              <input
                className="flex-1 px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
                placeholder="Enter access token"
                value={token}
                onChange={(e) => setToken(e.target.value)}
                type="password"
              />
              <button
                onClick={startConnection}
                disabled={!token}
                className={`px-6 py-2 rounded font-medium ${
                  token
                    ? "bg-blue-500 hover:bg-blue-600 text-white"
                    : "bg-gray-300 text-gray-500 cursor-not-allowed"
                }`}
              >
                Connect
              </button>
            </div>
          </div>
        )}

        {/* Messages */}
        <div className="flex-1 overflow-y-auto p-6 space-y-3">
          {messages.length === 0 ? (
            <div className="flex items-center justify-center h-full text-gray-400">
              <p>No messages yet. Start chatting!</p>
            </div>
          ) : (
            <>
              {messages.map((msg, i) => (
                <div
                  key={i}
                  className={`flex ${msg.isSender ? "justify-end" : "justify-start"}`}
                >
                  <div
                    className={`max-w-xs px-4 py-2 rounded-lg ${
                      msg.isSender
                        ? "bg-blue-500 text-white"
                        : "bg-gray-200 text-gray-800"
                    }`}
                  >
                    {msg.content}
                  </div>
                </div>
              ))}
              <div ref={messagesEndRef} />
            </>
          )}
        </div>

        {/* Message Input */}
        <div className="border-t px-6 py-4">
          <div className="flex gap-3">
            <input
              className="flex-1 px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-2 focus:ring-blue-500"
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
    </div>
  );
};

export default Chat;
