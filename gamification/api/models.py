from django.db import models

# Create your models here.
class User(models.Model):
    name=models.CharField(max_length=255)

class Objects(models.Model):
    item=models.CharField(max_length=255)
    point=models.IntegerField()

class Points(models.Model):
    user=models.ForeignKey(to=User, on_delete=models.CASCADE)
    points=models.IntegerField()
