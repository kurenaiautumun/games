# Generated by Django 4.2.2 on 2024-01-13 17:01

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('Quizing', '0007_score_name'),
    ]

    operations = [
        migrations.AddField(
            model_name='score',
            name='claim',
            field=models.BooleanField(default=False),
        ),
    ]
