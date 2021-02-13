namespace JellyfinJav.Api
{
    using System;
    using System.Text;

    /// <summary>A struct for representing a jav actress.</summary>
    public readonly struct Actress
    {
        /// <summary>The website-specific identifier.</summary>
        public readonly string Id;

        /// <summary>The actress' English name in either First-Last or Last-First order.</summary>
        public readonly string Name;

        /// <summary>The actress' birthday or null if unknown.</summary>
        public readonly DateTime? Birthdate;

        /// <summary>The actress' birthplace, normally will be just the prefecture or generic Japan.</summary>
        public readonly string? Birthplace;

        /// <summary>A url to a picture of the actress, or null if none was found.</summary>
        public readonly string? Cover;

        /// <summary>Initializes a new instance of the <see cref="Actress" /> struct.</summary>
        /// <param name="id">The actress' website-specific identifier.</param>
        /// <param name="name">The actress' English name.</param>
        /// <param name="birthdate">The actress' birthday.</param>
        /// <param name="birthplace">The actress' birthplace.</param>
        /// <param name="cover">A url to a picture of the actress.</param>
        public Actress(
            string id,
            string name,
            DateTime? birthdate,
            string? birthplace,
            string? cover)
        {
            this.Id = id;
            this.Name = name;
            this.Birthdate = birthdate;
            this.Birthplace = birthplace;
            this.Cover = cover;
        }

        /// <summary>Checks if two Actress objects are equal.</summary>
        /// <param name="a1">The first actress.</param>
        /// <param name="a2">The second actress.</param>
        public static bool operator ==(Actress a1, Actress a2)
        {
            return a1.Id == a2.Id &&
                   a1.Name == a2.Name &&
                   a1.Birthdate == a2.Birthdate &&
                   a1.Birthplace == a2.Birthplace &&
                   a1.Cover == a2.Cover;
        }

        /// <summary>Checks if two Actress objects are not equal.</summary>
        /// <param name="a1">The first actress.</param>
        /// <param name="a2">The second actress.</param>
        public static bool operator !=(Actress a1, Actress a2)
        {
            return !(a1 == a2);
        }

        /// <summary>Prints out each of the actress' public variables.</summary>
        /// <returns>All the public variables.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append("id: ").AppendLine(this.Id);
            sb.Append("name: ").AppendLine(this.Name);
            sb.Append("birthdate: ").AppendLine(this.Birthdate.ToString());
            sb.Append("birthplace: ").AppendLine(this.Birthplace);
            sb.Append("cover: ").AppendLine(this.Cover);

            return sb.ToString();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^
                   this.Name.GetHashCode() ^
                   this.Birthdate.GetHashCode() ^
                   this.Birthplace?.GetHashCode() ?? 0 ^
                   this.Cover?.GetHashCode() ?? 0;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Actress o && this == o;
        }
    }
}