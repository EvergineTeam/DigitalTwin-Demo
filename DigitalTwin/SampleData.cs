using Evergine.Mathematics;

namespace DigitalTwin
{
    public class SampleData
    {
        private readonly float[] sunAngles;
        private readonly float[] trackerAngles;
        private uint currentIndex;

        public SampleData(uint dataCount)
        {
            this.sunAngles = new float[dataCount];
            this.trackerAngles = new float[dataCount];
            this.currentIndex = 0;
            this.Populate(dataCount);
        }

        public (float sunAngle, float trackerAngle) Next()
        {
            var sunAngle = this.sunAngles[this.currentIndex];
            var trackerAngle = this.trackerAngles[this.currentIndex];
            this.currentIndex = (this.currentIndex + 1) % (uint)sunAngles.Length;
            return (sunAngle, trackerAngle);
        }

        private void Populate(uint dataCount)
        {
            //Fill angles
            int start = -90;
            int end = 90;
            for (int i = 0; i < dataCount; i++)
            {
                int value1 = (int)MathHelper.Lerp(start, end, (float)i / (float)dataCount);
                this.sunAngles[i] = MathHelper.ToRadians(value1);
                int value2 = (int)MathHelper.Lerp(start + 35, end - 35, (float)i / (float)dataCount);
                this.trackerAngles[i] = MathHelper.ToRadians(value2);
            }
        }
    }
}
