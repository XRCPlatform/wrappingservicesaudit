using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WrappingServicesAudit
{
    public class Money 
    {

        // for decimal.TryParse. None of the NumberStyles' composed values is useful for bitcoin style
        private const NumberStyles BitcoinStyle =
                          NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite
                        | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;


        /// <summary>
        /// Parse a bitcoin amount (Culture Invariant)
        /// </summary>
        /// <param name="bitcoin"></param>
        /// <param name="nRet"></param>
        /// <returns></returns>
        public static bool TryParse(string bitcoin, out Money nRet)
        {
            nRet = null;

            decimal value;
            if (!decimal.TryParse(bitcoin, BitcoinStyle, CultureInfo.InvariantCulture, out value))
            {
                return false;
            }

            try
            {
                nRet = new Money(value, MoneyUnit.XRC);
                return true;
            }
            catch (OverflowException)
            {
                return false;
            }
        }

        /// <summary>
        /// Parse a bitcoin amount (Culture Invariant)
        /// </summary>
        /// <param name="bitcoin"></param>
        /// <returns></returns>
        public static Money Parse(string bitcoin)
        {
            Money result;
            if (TryParse(bitcoin, out result))
            {
                return result;
            }
            throw new FormatException("Impossible to parse the string in a bitcoin amount");
        }

        long _Satoshis;
        public long Satoshi
        {
            get
            {
                return _Satoshis;
            }
            // used as a central point where long.MinValue checking can be enforced
            private set
            {
                CheckLongMinValue(value);
                _Satoshis = value;
            }
        }

        /// <summary>
        /// Get absolute value of the instance
        /// </summary>
        /// <returns></returns>
        public Money Abs()
        {
            var a = this;
            if (a < Money.Zero)
                a = -a;
            return a;
        }

        public Money(int satoshis)
        {
            Satoshi = satoshis;
        }

        public Money(uint satoshis)
        {
            Satoshi = satoshis;
        }

        public Money(long satoshis)
        {
            Satoshi = satoshis;
        }

        public Money(ulong satoshis)
        {
            // overflow check.
            // ulong.MaxValue is greater than long.MaxValue
            checked
            {
                Satoshi = (long)satoshis;
            }
        }

        public Money(decimal amount, MoneyUnit unit)
        {
            // sanity check. Only valid units are allowed
            CheckMoneyUnit(unit, "unit");
            checked
            {
                var satoshi = amount * (int)unit;
                Satoshi = (long)satoshi;
            }
        }


        /// <summary>
        /// Split the Money in parts without loss
        /// </summary>
        /// <param name="parts">The number of parts (must be more than 0)</param>
        /// <returns>The splitted money</returns>
        public IEnumerable<Money> Split(int parts)
        {
            if (parts <= 0)
                throw new ArgumentOutOfRangeException("Parts should be more than 0", "parts");
            long remain;
            long result = DivRem(_Satoshis, parts, out remain);

            for (int i = 0; i < parts; i++)
            {
                yield return Money.Satoshis(result + (remain > 0 ? 1 : 0));
                remain--;
            }
        }

        private static long DivRem(long a, long b, out long result)
        {
            result = a % b;
            return a / b;
        }

        public static Money FromUnit(decimal amount, MoneyUnit unit)
        {
            return new Money(amount, unit);
        }

        /// <summary>
        /// Convert Money to decimal (same as ToDecimal)
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public decimal ToUnit(MoneyUnit unit)
        {
            CheckMoneyUnit(unit, "unit");
            // overflow safe because (long / int) always fit in decimal
            // decimal operations are checked by default
            return (decimal)Satoshi / (int)unit;
        }
        /// <summary>
        /// Convert Money to decimal (same as ToUnit)
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public decimal ToDecimal(MoneyUnit unit)
        {
            return ToUnit(unit);
        }

        public static Money Coins(decimal coins)
        {
            // overflow safe.
            // decimal operations are checked by default
            return new Money(coins * COIN, MoneyUnit.Satoshi);
        }

        public static Money Bits(decimal bits)
        {
            // overflow safe.
            // decimal operations are checked by default
            return new Money(bits * CENT, MoneyUnit.Satoshi);
        }

        public static Money Cents(decimal cents)
        {
            // overflow safe.
            // decimal operations are checked by default
            return new Money(cents * CENT, MoneyUnit.Satoshi);
        }

        public static Money Satoshis(decimal sats)
        {
            return new Money(sats, MoneyUnit.Satoshi);
        }

        public static Money Satoshis(ulong sats)
        {
            return new Money(sats);
        }

        public static Money Satoshis(long sats)
        {
            return new Money(sats);
        }

        #region IEquatable<Money> Members

        public bool Equals(Money other)
        {
            if (other == null)
                return false;
            return _Satoshis.Equals(other._Satoshis);
        }

        public int CompareTo(Money other)
        {
            if (other == null)
                return 1;
            return _Satoshis.CompareTo(other._Satoshis);
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;
            Money m = obj as Money;
            if (m != null)
                return _Satoshis.CompareTo(m._Satoshis);
#if !(PORTABLE || NETCORE)
            return _Satoshis.CompareTo(obj);
#else
            return _Satoshis.CompareTo((long)obj);
#endif
        }

        #endregion

        public static Money operator -(Money left, Money right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");
            return new Money(checked(left._Satoshis - right._Satoshis));
        }
        public static Money operator -(Money left)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            return new Money(checked(-left._Satoshis));
        }
        public static Money operator +(Money left, Money right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");
            return new Money(checked(left._Satoshis + right._Satoshis));
        }
        public static Money operator *(int left, Money right)
        {
            if (right == null)
                throw new ArgumentNullException("right");
            return Money.Satoshis(checked(left * right._Satoshis));
        }

        public static Money operator *(Money right, int left)
        {
            if (right == null)
                throw new ArgumentNullException("right");
            return Money.Satoshis(checked(right._Satoshis * left));
        }
        public static Money operator *(long left, Money right)
        {
            if (right == null)
                throw new ArgumentNullException("right");
            return Money.Satoshis(checked(left * right._Satoshis));
        }
        public static Money operator *(Money right, long left)
        {
            if (right == null)
                throw new ArgumentNullException("right");
            return Money.Satoshis(checked(left * right._Satoshis));
        }

        public static Money operator /(Money left, long right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            return new Money(checked(left._Satoshis / right));
        }

        public static bool operator <(Money left, Money right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");
            return left._Satoshis < right._Satoshis;
        }
        public static bool operator >(Money left, Money right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");
            return left._Satoshis > right._Satoshis;
        }
        public static bool operator <=(Money left, Money right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");
            return left._Satoshis <= right._Satoshis;
        }
        public static bool operator >=(Money left, Money right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");
            return left._Satoshis >= right._Satoshis;
        }

        public static implicit operator Money(long value)
        {
            return new Money(value);
        }
        public static implicit operator Money(int value)
        {
            return new Money(value);
        }

        public static implicit operator Money(uint value)
        {
            return new Money(value);
        }

        public static implicit operator Money(ulong value)
        {
            return new Money(checked((long)value));
        }

        public static implicit operator long(Money value)
        {
            return value.Satoshi;
        }

        public static implicit operator ulong(Money value)
        {
            return checked((ulong)value.Satoshi);
        }

        public static implicit operator Money(string value)
        {
            return Money.Parse(value);
        }

        public override bool Equals(object obj)
        {
            Money item = obj as Money;
            if (item == null)
                return false;
            return _Satoshis.Equals(item._Satoshis);
        }
        public static bool operator ==(Money a, Money b)
        {
            if (Object.ReferenceEquals(a, b))
                return true;
            if (((object)a == null) || ((object)b == null))
                return false;
            return a._Satoshis == b._Satoshis;
        }

        public static bool operator !=(Money a, Money b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return _Satoshis.GetHashCode();
        }


        /// <summary>
        /// Returns a culture invariant string representation of Bitcoin amount
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(false, false);
        }

        /// <summary>
        /// Returns a culture invariant string representation of Bitcoin amount
        /// </summary>
        /// <param name="fplus">True if show + for a positive amount</param>
        /// <param name="trimExcessZero">True if trim excess zeroes</param>
        /// <returns></returns>
        public string ToString(bool fplus, bool trimExcessZero = true)
        {
            var fmt = string.Format("{{0:{0}{1}B}}",
                                    (fplus ? "+" : null),
                                    (trimExcessZero ? "2" : "8"));
            return string.Format(BitcoinFormatter.Formatter, fmt, _Satoshis);
        }


        static Money _Zero = new Money(0);
        public static Money Zero
        {
            get
            {
                return _Zero;
            }
        }

        /// <summary>
        /// Tell if amount is almost equal to this instance
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="dust">more or less amount</param>
        /// <returns>true if equals, else false</returns>
        public bool Almost(Money amount, Money dust)
        {
            if (amount == null)
                throw new ArgumentNullException("amount");
            if (dust == null)
                throw new ArgumentNullException("dust");
            return (amount - this).Abs() <= dust;
        }

        /// <summary>
        /// Tell if amount is almost equal to this instance
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="margin">error margin (between 0 and 1)</param>
        /// <returns>true if equals, else false</returns>
        public bool Almost(Money amount, decimal margin)
        {
            if (amount == null)
                throw new ArgumentNullException("amount");
            if (margin < 0.0m || margin > 1.0m)
                throw new ArgumentOutOfRangeException("margin", "margin should be between 0 and 1");
            var dust = Money.Satoshis((decimal)this.Satoshi * margin);
            return Almost(amount, dust);
        }

        public static Money Min(Money a, Money b)
        {
            if (a == null)
                throw new ArgumentNullException("a");
            if (b == null)
                throw new ArgumentNullException("b");
            if (a <= b)
                return a;
            return b;
        }

        public static Money Max(Money a, Money b)
        {
            if (a == null)
                throw new ArgumentNullException("a");
            if (b == null)
                throw new ArgumentNullException("b");
            if (a >= b)
                return a;
            return b;
        }

        private static void CheckLongMinValue(long value)
        {
            if (value == long.MinValue)
                throw new OverflowException("satoshis amount should be greater than long.MinValue");
        }

        private static void CheckMoneyUnit(MoneyUnit value, string paramName)
        {
            var typeOfMoneyUnit = typeof(MoneyUnit);
            if (!Enum.IsDefined(typeOfMoneyUnit, value))
            {
                throw new ArgumentException("Invalid value for MoneyUnit", paramName);
            }
        }

          
     

        public const long COIN = 100 * 1000 * 1000;
        public const long CENT = COIN / 100;
        public const long NANO = CENT / 100;


     
    }
    internal class BitcoinFormatter : IFormatProvider, ICustomFormatter
    {
        public static readonly BitcoinFormatter Formatter = new BitcoinFormatter();

        public object GetFormat(Type formatType)
        {
            return formatType == typeof(ICustomFormatter) ? this : null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (!this.Equals(formatProvider))
            {
                return null;
            }
            var i = 0;
            var plus = format[i] == '+';
            if (plus)
                i++;
            int decPos = 0;
            if (int.TryParse(format.Substring(i, 1), out decPos))
            {
                i++;
            }
            var unit = format[i];
            var unitToUseInCalc = MoneyUnit.XRC;
            switch (unit)
            {
                case 'B':
                    unitToUseInCalc = MoneyUnit.XRC;
                    break;
            }
            var val = Convert.ToDecimal(arg) / (int)unitToUseInCalc;
            var zeros = new string('0', decPos);
            var rest = new string('#', 10 - decPos);
            var fmt = plus && val > 0 ? "+" : string.Empty;

            fmt += "{0:0" + (decPos > 0 ? "." + zeros + rest : string.Empty) + "}";
            return string.Format(CultureInfo.InvariantCulture, fmt, val);
        }
    }


}
