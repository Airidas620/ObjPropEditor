using System;

namespace VisualPropertyEditor.Abstractions.Classes
{
    internal class NumericParser
    {
        /// <summary>
        /// Parses string value to given numeric type
        /// <para numericType>Number type </para>
        /// <para valueAsString>Number value as string</para>
        /// </summary>
        public static Object StringToNumericTypeValue(Type numericType, string valueAsString)
        {

            switch (Type.GetTypeCode(numericType))
            {
                case TypeCode.Byte:
                    Byte byteNumber;
                    if (Byte.TryParse(valueAsString, out byteNumber))
                    {
                        return byteNumber;
                    }
                    break;

                case TypeCode.Decimal:
                    Decimal decimalNumber;
                    if (Decimal.TryParse(valueAsString, out decimalNumber))
                    {
                        return decimalNumber;
                    }
                    break;

                case TypeCode.Double:
                    Double doubleNumber;
                    if (Double.TryParse(valueAsString, out doubleNumber))
                    {
                        return doubleNumber;
                    }
                    break;

                case TypeCode.Int16:
                    Int16 int16Number;
                    if (Int16.TryParse(valueAsString, out int16Number))
                    {
                        return int16Number;
                    }
                    break;

                case TypeCode.Int32:
                    Int32 int32Number;
                    if (Int32.TryParse(valueAsString, out int32Number))
                    {
                        return int32Number;
                    }
                    break;

                case TypeCode.Int64:
                    Int64 int64Number;
                    if (Int64.TryParse(valueAsString, out int64Number))
                    {
                        return int64Number;
                    }
                    break;

                case TypeCode.SByte:
                    sbyte sbyteNumber;
                    if (sbyte.TryParse(valueAsString, out sbyteNumber))
                    {
                        return sbyteNumber;
                    }
                    break;

                case TypeCode.Single:
                    Single SingleNumber;
                    if (Single.TryParse(valueAsString, out SingleNumber))
                    {
                        return SingleNumber;
                    }
                    break;

                case TypeCode.UInt16:
                    UInt16 uInt16Number;
                    if (UInt16.TryParse(valueAsString, out uInt16Number))
                    {
                        return uInt16Number;
                    }
                    break;

                case TypeCode.UInt32:
                    UInt32 uInt32Number;
                    if (UInt32.TryParse(valueAsString, out uInt32Number))
                    {
                        return uInt32Number;
                    }
                    break;

                case TypeCode.UInt64:
                    UInt64 uInt64Number;
                    if (UInt64.TryParse(valueAsString, out uInt64Number))
                    {
                        return uInt64Number;
                    }
                    break;
            }
            throw new Exception("Number parsing failed");
        }
    }
}
