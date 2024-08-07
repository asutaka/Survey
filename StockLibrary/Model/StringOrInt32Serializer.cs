﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Globalization;

namespace StockLibrary.Model
{
    public class StringOrInt32Serializer : SerializerBase<string>
    {
        public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonType = context.Reader.CurrentBsonType;
            switch (bsonType)
            {
                case BsonType.Null:
                    context.Reader.ReadNull();
                    return null;
                case BsonType.String:
                    return context.Reader.ReadString();
                case BsonType.Int32:
                    return context.Reader.ReadInt32().ToString(CultureInfo.InvariantCulture);
                default:
                    var message = string.Format($"Custom Cannot deserialize BsonString or BsonInt32 from BsonType {bsonType}");
                    throw new BsonSerializationException(message);
            }
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
        {
            if (value != null)
            {
                if (int.TryParse(value, out var result))
                {
                    context.Writer.WriteInt32(result);
                }
                else
                {
                    context.Writer.WriteString(value);
                }
            }
            else
            {
                context.Writer.WriteNull();
            }
        }
    }

    public class StringOrInt64Serializer : SerializerBase<string>
    {
        public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonType = context.Reader.CurrentBsonType;
            switch (bsonType)
            {
                case BsonType.Null:
                    context.Reader.ReadNull();
                    return null;
                case BsonType.String:
                    return context.Reader.ReadString();
                case BsonType.Int64:
                    return context.Reader.ReadInt64().ToString(CultureInfo.InvariantCulture);
                default:
                    var message = string.Format($"Custom Cannot deserialize BsonString or BsonInt64 from BsonType {bsonType}");
                    throw new BsonSerializationException(message);
            }
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
        {
            if (value != null)
            {
                if (int.TryParse(value, out var result))
                {
                    context.Writer.WriteInt32(result);
                }
                else
                {
                    context.Writer.WriteString(value);
                }
            }
            else
            {
                context.Writer.WriteNull();
            }
        }
    }

}
