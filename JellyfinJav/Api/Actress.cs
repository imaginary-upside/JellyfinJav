using System.Text;
using System;

namespace JellyfinJav.Api
{
    public readonly struct Actress
    {
        public readonly string Id;
        public readonly string Name;
        public readonly DateTime? Birthdate;
        public readonly string Birthplace;
        public readonly string Cover;

        public Actress(string id,
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

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^
                   Name.GetHashCode() ^
                   Birthdate.GetHashCode() ^
                   Birthplace.GetHashCode() ^
                   Cover.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Actress o && this == o;
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

            sb.AppendLine($"id: {Id}");
            sb.AppendLine($"name: {Name}");
            sb.AppendLine($"birthdate: {Birthdate}");
            sb.AppendLine($"birthplace: {Birthplace}");
            sb.AppendLine($"cover: {Cover}");

            return sb.ToString();
        }
    }
}