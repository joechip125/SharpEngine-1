﻿namespace SharpEngine
{
    public class Transform
    {
	    public Vector CurrentScale { get;  set; }
		public Vector Position { get;  set; }
		
		public Vector Rotation { get;  set; }
		public Matrix Matrix => Matrix.Translation(Position) * Matrix.Rotation(Rotation) * Matrix.Scale(CurrentScale);

		public Vector Forward => Matrix.Transform(Matrix, Vector.Forward, 0);
		
		public Vector Backward => Matrix.Transform(Matrix, Vector.Backward, 0);
		
		public Vector Left => Matrix.Transform(Matrix, Vector.Left, 0);
		public Vector Right => Matrix.Transform(Matrix, Vector.Right, 0);
		
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