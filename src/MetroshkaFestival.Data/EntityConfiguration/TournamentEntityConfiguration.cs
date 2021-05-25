﻿using MetroshkaFestival.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MetroshkaFestival.Data.EntityConfiguration
{
    public class TournamentEntityConfiguration : IEntityTypeConfiguration<Tournament>
    {
        public void Configure(EntityTypeBuilder<Tournament> entity)
        {
            entity.Property(x => x.Name)
                .IsRequired();

            entity.Property("CityId")
                .IsRequired();

            entity.HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}