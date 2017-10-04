using System;
using UnityEngine;

namespace HouraiTeahouse {

    /// <summary> A simple struct representing a range of real numbers. </summary>
    [Serializable]
    public struct IntRange {

        [SerializeField]
        int _min;

        [SerializeField]
        int _max;

        /// <summary> 
        /// The lower bound of the IntRange 
        /// </summary>
        public int Min {
            get { return _min; }
            set {
                if (value > Max) {
                    _min = _max;
                    _max = value;
                }
                else {
                    _min = value;
                }
            }
        }

        /// <summary> 
        /// The upper bound of the IntRange 
        /// </summary>
        public int Max {
            get { return _max; }
            set {
                if (value < Min) {
                    _max = _min;
                    _min = value;
                }
                else {
                    _min = value;
                }
            }
        }

        /// <summary> 
        /// Returns the width of the IntRange. Equal to the difference between Max and Min. 
        /// </summary>
        public int Width => Mathf.Abs(Max - Min);

        /// <summary> 
        /// Returns the center of the IntRange 
        /// </summary>
        public int Center => Lerp(0.5f);

        /// <summary> 
        /// Gets the full IntRange of real numbers, from negative infinity to positive infinity 
        /// </summary>
        public static IntRange FullRange => new IntRange(int.MinValue, int.MaxValue);

        public IntRange(int min, int max) {
            Argument.Check(min <= max);
            _min = min;
            _max = max;
        }

        /// <summary> Clamps a value to the IntRange </summary>
        /// <param name="x"> the value to clamp </param>
        /// <returns> the clamped value </returns>
        public int Clamp(int x) => Mathf.Clamp(x, Min, Max);

        public bool Contains(int value, bool inclusive = true) {
            if (inclusive)
                return value >= Min && value <= Max;
            return value > Min && value < Max;
        }

        /// <summary> 
        /// Selects a random floating point number contianed within the IntRange 
        /// </summary>
        /// <returns> a random number sampled from the IntRange, with uniform probabilty </returns>
        public int Random() {
            return UnityEngine.Random.Range(Min, Max);
        }

        /// <summary> 
        /// Linearly interpolates between the two extremes of the IntRange. 
        /// If val = 1, returns Max. If val = 0, returns /// Min. If betweeen, 
        /// the returned value is linearly proportional to the width of the IntRange. 
        /// </summary>
        public int Lerp(float val) => Mathf.FloorToInt(((Range) this).Lerp(val));

        public static IntRange FromValue(int val) => new IntRange(val, val);

        public static implicit operator IntRange(float i) => FromValue(Mathf.FloorToInt(i));
        public static implicit operator IntRange(int i) => FromValue(i);

        public static implicit operator IntRange(Range r) {
            return new IntRange(Mathf.FloorToInt(r.Min), Mathf.FloorToInt(r.Max));
        }

        public static IntRange operator +(IntRange r1, IntRange r2) {
            return new IntRange(r1.Min + r2.Min, r1.Max + r2.Max);
        }

        public static IntRange operator -(IntRange r1, IntRange r2) {
            return new IntRange(r1.Min - r2.Min, r1.Max - r2.Max);
        }

        public static IntRange operator *(int f, IntRange r) { return new IntRange(f * r.Min, f * r.Max); }

        public static IntRange operator *(IntRange r, int f) { return new IntRange(f * r.Min, f * r.Max); }

        public static IntRange operator /(IntRange r, int f) { return new IntRange(r.Min / f, r.Max / f); }

    }

    /// <summary> A simple struct representing a range of real numbers. </summary>
    [Serializable]
    public struct Range {

        [SerializeField]
        float _min;

        [SerializeField]
        float _max;

        /// <summary> The lower bound of the Range </summary>
        public float Min {
            get { return _min; }
            set {
                if (value > Max) {
                    _min = _max;
                    _max = value;
                } else {
                    _min = value;
                }
            }
        }

        /// <summary> The upper bound of the Range </summary>
        public float Max {
            get { return _max; }
            set {
                if (value < Min) {
                    _max = _min;
                    _min = value;
                } else {
                    _min = value;
                }
            }
        }

        /// <summary> Returns the width of the Range. Equal to the difference between Max and Min. </summary>
        public float Width => Mathf.Abs(Max - Min);

        /// <summary> Returns the center of the Range </summary>
        public float Center => Lerp(0.5f);

        /// <summary> Gets the full range of real numbers, from negative infinity to positive infinity </summary>
        public static Range FullRange => new Range(float.NegativeInfinity, float.PositiveInfinity);

        public static Range Positives => new Range(0f, float.PositiveInfinity);

        public static Range Negatives => new Range(float.NegativeInfinity, 0f);

        /// <summary> Creates an instance of Range. </summary>
        /// <param name="value"> </param>
        public Range(float value) {
            _min = value;
            _max = value;
        }

        /// <summary> Creates a instance of Range. </summary>
        /// <param name="min"> the </param>
        /// <param name="max"> </param>
        public Range(float min, float max) {
            _min = Mathf.Min(min, max);
            _max = Mathf.Max(min, max);
        }

        /// <summary> Clamps a value to the Range </summary>
        /// <param name="x"> the value to clamp </param>
        /// <returns> the clamped value </returns>
        public float Clamp(float x) {
            return Mathf.Clamp(x, Min, Max);
        }

        public bool Contains(float value, bool inclusive = true) {
            if (inclusive)
                return value >= Min && value <= Max;
            else
                return value > Min && value < Max;
        }

        /// <summary> Selects a random floating point number contianed within the Range </summary>
        /// <returns> a random number sampled from the range, with uniform probabilty </returns>
        public float Random() => UnityEngine.Random.Range(Min, Max);

        public float InverseLerp(float val) => Mathf.InverseLerp(Min, Max, Clamp(val));

        /// <summary> Linearly interpolates between the two extremes of the range. If val = 1, returns Max. If val = 0, returns
        /// Min. If betweeen, the returned value is linearly proportional to the width of the Range. </summary>
        /// <param name="val"> </param>
        /// <returns> </returns>
        public float Lerp(float val) {
            return Mathf.Lerp(Min, Max, val);
        }

        public override string ToString() => "<{0}:{1}>".With(Min, Max);

        public static implicit operator Range(float f) => new Range(f);
        public static implicit operator Range(int f) => new Range(f);
        public static implicit operator Range(IntRange r) => new Range(r.Min, r.Max);

        public static Range operator +(Range r1, Range r2) => new Range(r1.Min + r2.Min, r1.Max + r2.Max); 
        public static Range operator -(Range r1, Range r2) => new Range(r1.Min - r2.Min, r1.Max - r2.Max); 
        public static Range operator *(float f, Range r) =>new Range(f * r.Min, f * r.Max);
        public static Range operator *(Range r, float f) => new Range(f * r.Min, f * r.Max);
        public static Range operator /(Range r, float f) => new Range(r.Min / f, r.Max / f);

    }

}