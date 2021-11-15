using System;
using System.Runtime.InteropServices;
using static OpenGL.Gl;

namespace SharpEngine {
	public abstract class Shape {
            
		public Vertex[] vertices;
		Matrix transform = Matrix.Identity;
		uint vertexArray;
		uint vertexBuffer;

		public float CurrentScale { get; protected set; }

		public Material material;
            
		public Shape(Vertex[] vertices, Material material) {
			this.vertices = vertices;
			this.material = material;
			LoadTriangleIntoBuffer();
			this.CurrentScale = 1f;
		}
		
		 void LoadTriangleIntoBuffer() {
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

		public Vector GetMinBounds() {
			var min = this.vertices[0].position;
			for (var i = 1; i < this.vertices.Length; i++) {
				min = Vector.Min(min, this.vertices[i].position);
			}
			return min;
		}
            
		public Vector GetMaxBounds() {
			var max = this.vertices[0].position;
			for (var i = 1; i < this.vertices.Length; i++) {
				max = Vector.Max(max, this.vertices[i].position);
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
		
		public Vector GetCenter() {
			return (GetMinBounds() + GetMaxBounds()) / 2;
		}

		public void Scale(float multiplier) {

		}

		public void Move(Vector direction) {
			this.transform *= Matrix.Translation(direction);
		}

		public unsafe void Render() {
			this.material.Use();
			this.material.SetTransform(this.transform);
			glBindVertexArray(vertexArray);
			glBindBuffer(GL_ARRAY_BUFFER, this.vertexBuffer);
			fixed (Vertex* vertex = &this.vertices[0]) {
				glBufferData(GL_ARRAY_BUFFER, Marshal.SizeOf<Vertex>() * this.vertices.Length, vertex, GL_DYNAMIC_DRAW);
			}
			glDrawArrays(GL_TRIANGLES, 0, this.vertices.Length);
			glBindVertexArray(0);
		}

		public void Rotate(float rotation) {
			
		}
	}
	
	
	public class Triangle : Shape
	{
		private float Width;
		private float Height;
		private Vector Position;
		public Material material;
        
		public Triangle(Vertex[] vertices, Material material) : base( new Vertex []{ 
			new Vertex(new Vector(-0.5f, -0.5f), Color.Red),
			new Vertex(new Vector(0.5f, -0.5f), Color.Green),
			new Vertex(new Vector(0f, 0.5f), Color.Blue)}, material)
		{
			this.vertices = vertices;
			this.material = material;
			this.CurrentScale = 1f;
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
			Move(position);
		}

	}
	
	public class Rectangle : Shape
	{
		public  Vertex[] Vertices;
		private float Width;
		private float Height;
		private Vector Position;
        
     
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
			Move(position);
		}
	}
    
	public class Circle : Shape
	{
		public  Vertex[] Vertices;
		private float Radius;
		private Vector Position;
        
     
		public Circle(float radius, Vector position, Material material) : base (new Vertex []{ 
			new Vertex(new Vector(-0.5f, -0.5f), Color.Red),
		}, material)
		{

			Radius = radius;
			Position = position;
            
			AddVertices(16);
			SetVectorsCircle(Radius); 
			Move(position);
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