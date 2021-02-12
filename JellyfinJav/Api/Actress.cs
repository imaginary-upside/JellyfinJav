#pragma warning disable SA1600

namespace JellyfinJav.Api
{
    using System;
    using System.Text;

    public readonly struct Actress
    {
        public readonly string Id;
        public readonly string Name;
        public readonly DateTime? Birthdate;
        public readonly string Birthplace;
        public readonly string Cover;

        public Actress(
            string id,
            string name,
            DateTime? birthdate,
            string birthplace,
            string cover)
        {
            this.Id = id;
            this.Name = name;
            this.Birthdate = birthdate;
            this.Birthplace = birthplace;
            this.Cover = cover;
        }

        public static bool operator ==(Actress a1, Actress a2)
        {
            return a1.Id == a2.Id &&
                   a1.Name == a2.Name &&
                   a1.Birthdate == a2.Birthdate &&
                   a1.Birthplace == a2.Birthplace &&
                   a1.Cover == a2.Cover;
        }

        public static bool operator !=(Actress a1, Actress a2)
        {
            return !(a1 == a2);
        }

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

        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^
                   this.Name.GetHashCode() ^
                   this.Birthdate.GetHashCode() ^
                   this.Birthplace.GetHashCode() ^
                   this.Cover.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Actress o && this == o;
        }
    }
}