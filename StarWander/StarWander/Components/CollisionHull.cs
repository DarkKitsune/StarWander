using System;
using System.Linq;
using System.Collections.Generic;
using VulpineLib.Geometry;
using VulpineLib.Util;

namespace StarWander.Components
{
    public class CollisionHull : Component
    {
        private static List<CollisionHull>[] TeamCollisionHulls;

        static CollisionHull()
        {
            var teams = Enum.GetValues(typeof(Team));
            TeamCollisionHulls = new List<CollisionHull>[teams.Length];
            foreach (Team team in teams)
            {
                TeamCollisionHulls[(int)team] = new List<CollisionHull>();
            }
        }

        private static List<CollisionHull> GetTeamList(Team team)
        {
            return TeamCollisionHulls[(int)team];
        }

        /// <summary>
        /// The collision hull's team
        /// </summary>
        public Team Team { get; }

        /// <summary>
        /// The shapes making up the hull
        /// </summary>
        private List<CollisionShape> Shapes { get; } = new List<CollisionShape>();

        public CollisionHull(GameObject parent, Team team) : base(parent)
        {
            Team = team;
        }

        protected override void OnStart()
        {
            base.OnStart();
            GetTeamList(Team).Add(this);
        }

        protected override void OnEnd()
        {
            base.OnEnd();
            GetTeamList(Team).Remove(this);
        }

        /// <summary>
        /// Add a collision shape
        /// </summary>
        /// <param name="shape"></param>
        public void AddShape(CollisionShape shape)
        {
            Shapes.Add(shape);
        }

        /// <summary>
        /// Remove a collision shape
        /// </summary>
        /// <param name="shape"></param>
        public void RemoveShape(CollisionShape shape)
        {
            Shapes.Remove(shape);
        }

        /// <summary>
        /// Clear all collision shapes
        /// </summary>
        public void Clear()
        {
            Shapes.Clear();
        }

        /// <summary>
        /// Check if the hull is colliding with another hull
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsCollidingWith(CollisionHull other)
        {
            foreach (var shape1 in Shapes)
            {
                foreach (var shape2 in other.Shapes)
                {
                    if (shape1.IsCollidingWith(shape2))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get all collisions from a specific team
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public IEnumerable<CollisionHull> GetCollisions(Team team)
        {
            return GetTeamList(team).Where(IsCollidingWith);
        }
    }
}
