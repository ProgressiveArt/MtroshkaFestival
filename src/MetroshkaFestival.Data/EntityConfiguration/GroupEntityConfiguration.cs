﻿using MetroshkaFestival.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MetroshkaFestival.Data.EntityConfiguration
{
    public class GroupEntityConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> entity)
        {
            entity.Property(x => x.Name)
                .IsRequired();

            entity.Property("AgeCategoryId")
                .IsRequired();

            entity.Property(x => x.IsPlayOff)
                .IsRequired()
                .HasDefaultValue(false);

            entity.HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}