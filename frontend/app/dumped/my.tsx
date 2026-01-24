"use client";
import React, { FunctionComponent, useState } from "react";

type InputProps = {
  labelText?: string;
  type: string;
  placeholder: string;
  value: string;
  onChange: (value: string) => void;
};

type SendButtonProps = {
  width: string;
  handleSendMessage: React.MouseEventHandler<HTMLButtonElement>;
};

const Input = ({
  labelText,
  type,
  placeholder,
  value,
  onChange,
}: InputProps) => {
  return (
    <>
      {labelText && <label>{labelText}</label>}
      <input
        className="p-2 border w-full"
        type={type}
        placeholder={placeholder}
        value={value}
        onChange={(e) => onChange(e.target.value)}
      />
    </>
  );
};

const SendButton = ({ width, handleSendMessage }: SendButtonProps) => {
  return (
    <button
      onClick={handleSendMessage}
      className={`cursor-pointer p-2 bg-blue-500 text-white border-0 ${width}`}
    >
      Send Message
    </button>
  );
};

type MessageProps = {
  textAlign: string;
  message: string;
};

const Message = ({ textAlign, message }: MessageProps) => {
  return <div className={`bg-blue-400 p-2 ` + textAlign}>{message}</div>;
};

const Chat = () => {
  const [token, setToken] = useState<string>("");
  const [message, setMessage] = useState<string>("");
  const [messages, setMessages] = useState<string[]>([]);

  const handleSendMessage = () => {
    if (message.trim().length === 0) return;
    setMessages([...messages, message]);
    setMessage("");
  };

  return (
    <main className="border flex flex-col gap-10 p-5  w-full">
      <section className="flex flex-col justify-center ">
        <Input
          labelText="Access Token"
          type="text"
          placeholder="Enter your access token here . . ."
          value={token}
          onChange={(val) => setToken(val)}
        />
      </section>
      {/* Render messages here */}
      <section className="flex flex-col w-full gap-3">
        {messages.map((m, i) => {
          return (
            <Message
              key={i}
              message={m}
              textAlign={i % 2 == 0 ? "text-right" : "text-left"}
            />
          );
        })}
      </section>
      <section className="flex sm:flex-row flex-col w-full gap-2">
        <Input
          type="text"
          placeholder="Enter Message "
          value={message}
          onChange={(val) => setMessage(val)}
        />
        <SendButton width="w-[25%]" handleSendMessage={handleSendMessage} />
      </section>{" "}
    </main>
  );
};

export default Chat;
