using System;
using System.Runtime.InteropServices;
using static OpenGL.Gl;

namespace SharpEngine {
	public abstract class Shape 
	{
            
		public Vertex[] vertices;
		uint vertexArray;
		uint vertexBuffer;
		public Transform Transform { get; }
		public Material material;
		
		float mass = 1;
		float massInverse = 1;

		public float Mass {
			get => this.mass;
			set {
				this.mass = value;
				this.massInverse = float.IsPositiveInfinity(value) ? 0f : 1f / value;
			}
		}

		public float MassInverse => this.massInverse;

		public float gravityScale = 1f;
		public Vector velocity; // momentum = product of velocity and mass
		public Vector linearForce;

		public Shape(Vertex[] vertices, Material material) 
		{
			this.vertices = vertices;
			this.material = material;
			LoadShapeIntoBuffer();

			Transform = new Transform();
		}

		public void SetColor(Color color)
		{
			for (int i = 0; i < vertices.Length; i++)
			{
				vertices[i].color = color;
			}
		}
			
		
		 void LoadShapeIntoBuffer() 
		 {
			vertexArray = glGenVertexArray();
			vertexBuffer = glGenBuffer();
			glBindVertexArray(vertexArray);
			glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
			glVertexAttribPointer(0, 3, GL_FLOAT, false, Marshal.SizeOf<Vertex>(), Marshal.OffsetOf(typeof(Vertex), nameof(Vertex.position)));
			glVertexAttribPointer(1, 4, GL_FLOAT, false, Marshal.SizeOf<Vertex>(), Marshal.OffsetOf(typeof(Vertex), nameof(Vertex.color)));
			glEnableVertexAttribArray(0);
			glEnableVertexAttribArray(1);
			glBindVertexArray(0);
		}

		public Vector GetMinBounds() 
		{
			var min = Transform.Matrix * vertices[0].position;
			for (var i = 1; i < vertices.Length; i++) 
			{
				min = Vector.Min(min, Transform.Matrix * vertices[i].position);
			}
			return min;
		}
            
		public Vector GetMaxBounds() 
		{
			var max = Transform.Matrix * vertices[0].position;
			for (var i = 1; i < vertices.Length; i++) 
			{
				max = Vector.Max(max,  Transform.Matrix * vertices[i].position);
			}

			return max;
		}

		public void AddVertices(int numberToAdd)
		{
			Vertex[] newVert = new Vertex[vertices.Length + numberToAdd];

			for (int i = 0; i < vertices.Length + numberToAdd; i++)
			{
				newVert[i] = new Vertex(new Vector(0, 0), Color.Blue);
			}
		}

		public void SetVectorPositions(float Width, float Height, float startDegree)
		{
			float degreeIncrement = 360 / vertices.Length;
			float degrees = startDegree;

			for (int i = 0; i < vertices.Length; i++)
			{
				float cos = MathF.Cos((MathF.PI / 180) * degrees);
				float sin = MathF.Sin((MathF.PI / 180) * degrees);

				vertices[i].position.x = cos * Width / 2;
				vertices[i].position.y = sin * Height / 2;

				degrees += degreeIncrement;
			}
		}
        
		public void SetVectorsCircle(float radius)
		{
			float degreeIncrement = 360 / vertices.Length;
			float degrees = 0;

			for (int i = 0; i < vertices.Length; i++)
			{
				float cos = MathF.Cos((MathF.PI / 180) * degrees);
				float sin = MathF.Sin((MathF.PI / 180) * degrees);

				vertices[i].position.x = cos * radius;
				vertices[i].position.y = sin * radius;

				degrees += degreeIncrement;
			}
		}
		
		public Vector GetCenter()
		{
			return (GetMinBounds() + GetMaxBounds()) / 2;
		}

	
		public unsafe void Render() 
		{
			material.Use();
			material.SetTransform(Transform.Matrix);
			glBindVertexArray(vertexArray);
			glBindBuffer(GL_ARRAY_BUFFER, vertexBuffer);
			fixed (Vertex* vertex = &vertices[0]) 
			{
				glBufferData(GL_ARRAY_BUFFER, Marshal.SizeOf<Vertex>() * vertices.Length, vertex, GL_DYNAMIC_DRAW);
			}
			glDrawArrays(GL_TRIANGLES, 0, vertices.Length);
			glBindVertexArray(0);
		}

		public void Rotate(float rotation) 
		{
			
		}
	}
	
	
	public class Triangle : Shape
	{
		private float Width;
		private float Height;
		private Vector Position;
		public Material material;
        
		public Triangle(Material material) : base(CreateTriangle(), material) {
		}

		static Vertex[] CreateTriangle() {
			const float scale = .1f;
			float height = MathF.Sqrt(0.75f) * scale;
			return new Vertex[] {
				new Vertex(new Vector(-scale, -height/2), Color.Red),
				new Vertex(new Vector(scale, -height/2), Color.Green),
				new Vertex(new Vector(0f, height), Color.Blue)
			};
		}
		
		public Triangle(Vertex[] vertices, Material material) : base( new Vertex []{ 
			new Vertex(new Vector(-0.5f, -0.5f), Color.Red),
			new Vertex(new Vector(0.5f, -0.5f), Color.Green),
			new Vertex(new Vector(0f, 0.5f), Color.Blue)}, material)
		{
			
		}
        
		public Triangle(float width, float height, Vector position, Material material) : base( new Vertex []{ 
			new Vertex(new Vector(-0.5f, -0.5f), Color.Red),
			new Vertex(new Vector(0.5f, -0.5f), Color.Green),
			new Vertex(new Vector(0f, 0.5f), Color.Blue)}, material)
		{
			Width = width;
			Height = height;
			Position = position;
			SetVectorPositions(Width, Height, 90);
		//	Move(position);
		}

	}
	
	public class Rectangle : Shape
	{
		public  Vertex[] Vertices;
		private float Width;
		private float Height;
		private Vector Position;
        
		public Rectangle(Material material) : base(CreateRectangle(), material) {
		}

		static Vertex[] CreateRectangle() {
			const float scale = .1f;
			return new Vertex[] {
				new Vertex(new Vector(-scale, -scale), Color.Red),
				new Vertex(new Vector(scale, -scale), Color.Green),
				new Vertex(new Vector(-scale, scale), Color.Blue),
				new Vertex(new Vector(scale, -scale), Color.Green),
				new Vertex(new Vector(scale, scale), Color.Red),
				new Vertex(new Vector(-scale, scale), Color.Blue)
			};
		}
		
     
		public Rectangle(float width, float height, Vector position, Material material) : base (new Vertex []{ 
			new Vertex(new Vector(-0.5f, -0.5f), Color.Red),
			new Vertex(new Vector(0.5f, -0.5f), Color.Green),
			new Vertex(new Vector(0.5f, 0.5f), Color.Blue),
			new Vertex(new Vector(-0.5f, 0.5f), Color.Blue)
		}, material)
		{
			Width = width;
			Height = height;
			Position = position;
			SetVectorPositions(Width, Height, 45);
		//	Move(position);
		}
	}
    
	public class Circle : Shape
	{
		public  Vertex[] Vertices;
		public float Radius => .1f;
		private Vector Position;

		public Circle(Material material) : base(CreateCircle(), material) 
		{
		}
		
		static Vertex[] CreateCircle()
		{
			const int numberOfSegments = 32;
			const int verticesPerSegment = 3;
			const float scale = .1f;
			Vertex[] result = new Vertex[numberOfSegments * verticesPerSegment];
			const float circleRadians = MathF.PI * 2;
			var oldAngle = 0f;
			for (int i = 0; i < numberOfSegments; i++)
			{
				int currentVertex = i * verticesPerSegment;
				var newAngle = circleRadians / numberOfSegments * (i + 1);
				result[currentVertex++] = new Vertex(new Vector(), Color.Blue);
				result[currentVertex++] = new Vertex(new Vector(MathF.Cos(oldAngle), MathF.Sin(oldAngle)) * scale,
					Color.Green);
				result[currentVertex] =
					new Vertex(new Vector(MathF.Cos(newAngle), MathF.Sin(newAngle)) * scale, Color.Red);
				oldAngle = newAngle;
			}

			return result;
		}

		public Circle(float radius, Vector position, Material material) : base (new Vertex []{ 
			new Vertex(new Vector(-0.5f, -0.5f), Color.Red)
		}, material)
		{
			
			Position = position;
            
			AddVertices(16);
			SetVectorsCircle(Radius); 
		//	Transform.Move(position);
		}
	}
    
	public class Cone : Shape
	{
		public  Vertex[] Vertices;
		private float Radius;
		private float Angle;
		private Vector Position;
        
     
		public Cone(float radius, float angle, Vector position, Material material) : base (new Vertex []{ 
			new Vertex(new Vector(-0.5f, -0.5f), Color.Red),
			new Vertex(new Vector(0.5f, -0.5f), Color.Green),
			new Vertex(new Vector(0f, 0.5f), Color.Blue)}, material)
		{
			Angle = angle;
			Radius = radius;
			Position = position;
		}
	}
}