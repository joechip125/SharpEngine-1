namespace SharpEngine
{
    public class Transform
    {
	    public Vector CurrentScale { get; private set; }
		public Vector Position { get; private set; }
		
		public Vector Rotation { get; private set; }
		public Matrix Matrix => Matrix.Translation(Position) * Matrix.Rotation(Rotation) * Matrix.Scale(CurrentScale);
		
		public Transform()
		{
			CurrentScale = new Vector(1, 1, 1);
		}

		public void Scale(float multiplier)
		{
			CurrentScale *= multiplier;
		}

		public void Move(Vector direction) 
		{
			Position += direction;
		}

		public void Rotate(float rotation)
		{
			var rotationT = Rotation;
			rotationT.z += rotation;
			Rotation = rotationT;
		}
    }
}