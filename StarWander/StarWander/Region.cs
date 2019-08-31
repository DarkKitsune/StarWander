using System;
using System.Collections.Generic;
using VulpineLib.Util;
using StarWander.GFX;

namespace StarWander
{
    public class Region
    {
        /// <summary>
        /// Region size
        /// </summary>
        public Vector3<decimal> Size { get; }

        /// <summary>
        /// Region center
        /// </summary>
        public Vector3<decimal> Center { get; }

        /// <summary>
        /// Region min corner
        /// </summary>
        public Vector3<decimal> Min => Center - Size / 2m;

        /// <summary>
        /// Region max corner
        /// </summary>
        public Vector3<decimal> Max => Center + Size / 2m;

        /// <summary>
        /// Region draw bounding box (only draw if camera is within these bounds)
        /// </summary>
        public Vector3<decimal> DrawBoundingBox { get; }

        /// <summary>
        /// Region min corner
        /// </summary>
        public Vector3<decimal> DrawMin => Center - DrawBoundingBox / 2m;

        /// <summary>
        /// Region max corner
        /// </summary>
        public Vector3<decimal> DrawMax => Center + DrawBoundingBox / 2m;

        /// <summary>
        /// Region contents
        /// </summary>
        public List<GameObject> Contents { get; } = new List<GameObject>();

        public Region(Vector3<decimal> center, Vector3<decimal> size, Vector3<decimal> drawBoundingBox)
        {
            Center = center;
            Size = size;
            DrawBoundingBox = drawBoundingBox;
        }

        /// <summary>
        /// Generate objects here
        /// </summary>
        public virtual void Populate()
        {
        }

        /// <summary>
        /// Clear and delete all objects inside
        /// </summary>
        public void Clear()
        {
            foreach (var obj in Contents.ToArray())
                obj.Delete();
            ClearNoDelete();
        }

        /// <summary>
        /// Clear objects without deleting them
        /// </summary>
        public void ClearNoDelete()
        {
            Contents.Clear();
        }

        /// <summary>
        /// Add an object
        /// </summary>
        /// <param name="obj"></param>
        public void Add(GameObject obj)
        {
            Contents.Add(obj);
            obj.Start();
        }

        /// <summary>
        /// Remove an object
        /// </summary>
        /// <param name="obj"></param>
        public void Remove(GameObject obj)
        {
            Contents.Remove(obj);
        }

        /// <summary>
        /// Update the region
        /// </summary>
        /// <param name="time"></param>
        /// <param name="delta"></param>
        public void Update(double time, double delta)
        {
            OnUpdate(time, delta);
        }

        /// <summary>
        /// Called when updating the region
        /// </summary>
        protected virtual void OnUpdate(double time, double delta)
        {
            foreach (var obj in Contents)
                obj.Update(time, delta);
        }

        /// <summary>
        /// Draw the region
        /// </summary>
        /// <param name="time"></param>
        /// <param name="delta"></param>
        public void Draw(double time, double delta)
        {
            var cam = GameObjects.CameraObject.Current;
            if (cam == null)
                throw new NullReferenceException();

            if (cam.IsWithin(this))
            {
                Camera.Push();
                cam.SetCamera(
                        this,
                        SGameWindow.Main!.AspectRatio,
                        (float)(cam.Region.DrawBoundingBox.Length * 0.00001),
                        (float)cam.Region.DrawBoundingBox.Length
                    );
                OnDraw(time, delta);
                Camera.Pop();
            }
        }

        /// <summary>
        /// Called when drawing the region
        /// </summary>
        protected virtual void OnDraw(double time, double delta)
        {
            foreach (var obj in Contents)
                obj.Draw(time, delta);
        }
    }
}
