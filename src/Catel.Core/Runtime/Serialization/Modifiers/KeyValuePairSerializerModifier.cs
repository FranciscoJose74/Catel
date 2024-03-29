﻿namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using Reflection;

    /// <summary>
    /// Serializer modifier that supports key value pairs automatically.
    /// </summary>
    public class KeyValuePairSerializerModifier : SerializerModifierBase
    {
        private const string Prefix = "CTL_KeyValuePair";
        private const string Splitter = "<|::|>";

        /// <summary>
        /// Serializes the member.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        public override void SerializeMember(ISerializationContext context, MemberValue memberValue)
        {
            base.SerializeMember(context, memberValue);

            var value = memberValue.Value;
            if (value is not null)
            {
                var valueType = value.GetType();
                if (valueType.IsGenericTypeEx())
                {
                    if (valueType.GetGenericTypeDefinitionEx() == typeof(KeyValuePair<,>))
                    {
                        var keyProperty = valueType.GetPropertyEx("Key")!;
                        var valueProperty = valueType.GetPropertyEx("Value")!;

                        var kvpKey = keyProperty.GetValue(value, null);
                        var kvpValue = valueProperty.GetValue(value, null);

#pragma warning disable HAA0101 // Array allocation for params parameter
                        var finalValue = string.Format("{0}{1}{2}{1}{3}{1}{4}{1}{5}", Prefix, Splitter,
                            keyProperty.PropertyType, valueProperty.PropertyType,
                            ObjectToStringHelper.ToString(kvpKey), ObjectToStringHelper.ToString(kvpValue));
#pragma warning restore HAA0101 // Array allocation for params parameter

                        memberValue.Value = finalValue;
                    }
                }
            }
        }

        /// <summary>
        /// Deserializes the member.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        public override void DeserializeMember(ISerializationContext context, MemberValue memberValue)
        {
            var valueAsString = memberValue.Value as string;
            if (!string.IsNullOrEmpty(valueAsString))
            {
                if (valueAsString.StartsWith(Prefix))
                {
                    var splittedValues = valueAsString.Split(new[] { Splitter }, StringSplitOptions.None);

                    var keyValuePairType = typeof(KeyValuePair<,>);
                    var keyType = TypeCache.GetTypeWithoutAssembly(splittedValues[1], allowInitialization: false);
                    if (keyType is null)
                    {
                        throw new CatelException($"Cannot find key type '{splittedValues[1]}'");
                    }

                    var valueType = TypeCache.GetTypeWithoutAssembly(splittedValues[2], allowInitialization: false);
                    if (valueType is null)
                    {
                        throw new CatelException($"Cannot find value type '{splittedValues[2]}'");
                    }

                    var keyValue = splittedValues[3];
                    var valueValue = splittedValues[4];

#pragma warning disable HAA0101 // Array allocation for params parameter
                    var keyValuePairGenericType = keyValuePairType.MakeGenericType(keyType, valueType);
#pragma warning restore HAA0101 // Array allocation for params parameter

                    var key = StringToObjectHelper.ToRightType(keyType, keyValue);
                    var value = StringToObjectHelper.ToRightType(valueType, valueValue);

                    var keyValuePair = Activator.CreateInstance(keyValuePairGenericType, new [] { key, value });

                    memberValue.Value = keyValuePair;
                }
            }

            base.DeserializeMember(context, memberValue);
        }
    }
}
