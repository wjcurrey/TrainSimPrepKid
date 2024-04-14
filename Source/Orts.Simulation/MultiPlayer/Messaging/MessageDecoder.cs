﻿using System.IO.Pipelines;
using System.Threading.Tasks;

using MemoryPack;

using MultiPlayer.Shared;

namespace Orts.Simulation.MultiPlayer.Messaging
{
    internal static class MessageDecoder
    {
        public static MultiPlayerMessageContent DecodeMessage(MultiPlayerMessage message)
        {
            return message.MessageType switch
            {
                MessageType.Legacy => new LegacyMessage { Payload = message.Payload },
                MessageType.Server => new ServerMessage() { Dispatcher = message.PayloadAsString },
                MessageType.Lost => new LostMessage() { User = message.PayloadAsString },
                MessageType.Chat => MemoryPackSerializer.Deserialize<ChatMessage>(message.Payload),
                MessageType.Aider => MemoryPackSerializer.Deserialize<AiderMessage>(message.Payload),
                MessageType.Quit => MemoryPackSerializer.Deserialize<QuitMessage>(message.Payload),
                MessageType.TimeCheck => MemoryPackSerializer.Deserialize<TimeCheckMessage>(message.Payload),
                _ => throw new ProtocolException($"Unknown Message type {message.MessageType}"),
            };
        }

        public static async Task<MultiPlayerMessage> EncodeMessage(MultiPlayerMessageContent message)
        {
            ReadResult resultBuffer;
            Pipe bufferPipe = new Pipe();
            MessageType messageType = MessageType.Unknown;

            switch (message)
            {
                case ServerMessage dispatcherMessage:
                    MemoryPackSerializer.Serialize(bufferPipe.Writer, dispatcherMessage);
                    messageType = MessageType.Server;
                    break;
                case ChatMessage chatMessage:
                    MemoryPackSerializer.Serialize(bufferPipe.Writer, chatMessage);
                    messageType = MessageType.Chat;
                    break;
                case AiderMessage aiderMessage:
                    MemoryPackSerializer.Serialize(bufferPipe.Writer, aiderMessage);
                    messageType = MessageType.Aider;
                    break;
                case QuitMessage quitMessage:
                    MemoryPackSerializer.Serialize(bufferPipe.Writer, quitMessage);
                    messageType = MessageType.Quit;
                    break;
                case TimeCheckMessage timeCheckMessage:
                    MemoryPackSerializer.Serialize(bufferPipe.Writer, timeCheckMessage);
                    messageType = MessageType.TimeCheck;
                    break;
            }

            _ = await bufferPipe.Writer.FlushAsync().ConfigureAwait(false);
            resultBuffer = await bufferPipe.Reader.ReadAsync().ConfigureAwait(false);
            return new MultiPlayerMessage() { MessageType = messageType, Payload = resultBuffer.Buffer };
        }
    }
}
