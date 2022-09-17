from .models import Objects,Points,User
from rest_framework import serializers

class UserSerializer(serializers.ModelSerializer):
    class Meta:
        model=User
        fields="__all__"

class ObjectsSerializers(serializers.ModelSerializer):
    class Meta:
        model=Objects
        fields="__all__"

class PointsSerializers(serializers.ModelSerializer):
    class Meta:
        model=Points
        fields="__all__"