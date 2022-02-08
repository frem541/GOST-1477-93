using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScrewNX
{
    public class Screw
    {
        private readonly double _faskSize, _outerDiameter, _innerDiameter, _slotWidth, _threadPitch, _minStandartLength;

        public static readonly Dictionary<string, Screw> Table_GOST_1477_93;

        public double FaskSize { get { return _faskSize; } }
        public double OuterDiameter { get { return _outerDiameter; } }
        public double InnerDiameter { get { return _innerDiameter; } }
        public double SlotWidth { get { return _slotWidth; } }
        public double ThreadPitch { get { return _threadPitch; } }
        public double MinStandartLength { get { return _minStandartLength; } }

        public Screw(double faskSize, double outerDiameter, double innerDiameter, double slotWidth, double threadPitch, double minStandartLength)
        {
            _faskSize = faskSize;
            _outerDiameter = outerDiameter;
            _innerDiameter = innerDiameter;
            _slotWidth = slotWidth;
            _threadPitch = threadPitch;
            _minStandartLength = minStandartLength;
        }
        static Screw()
        {
            Table_GOST_1477_93 = new Dictionary<string, Screw>
            {
                {"M1", new Screw(0.2, 1, 0.75, 0.2, 0.25, 2) },
                {"M1.2", new Screw(0.2, 1.2, 0.95, 0.2, 0.25, 2) },
                {"M1.6", new Screw(0.3, 1.6, 1.25, 0.25, 0.35, 2.5) },
                {"M2", new Screw(0.3, 2, 1.6, 0.25, 0.4, 3) },
                {"M2.5", new Screw(0.3, 2.5, 2.05, 0.4, 0.45, 4) },
                {"M3", new Screw(0.5, 3, 2.5, 0.4, 0.5, 4) },
                {"M3.5", new Screw(0.5, 3.5, 2.9, 0.5, 0.6, 5) },
                {"M4", new Screw(0.5, 4, 3.3, 0.6, 0.7, 5) },
                {"M5", new Screw(1, 5, 4.2, 0.8, 0.8, 6) },
                {"M6", new Screw(1, 6, 4.95, 1, 1, 8) },
                {"M8", new Screw(1.6, 8, 6.7, 1.2, 1.25, 8) },
                {"M10", new Screw(1.6, 10, 8.43, 1.6, 1.5, 10) },
                {"M12", new Screw(1.6, 12, 10.2, 2, 1.75, 12) }
            };
        }
    }
}
