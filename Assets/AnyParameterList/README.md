# AnyParameterList for Unity

## What is this

 AnyParameterList is a container component library for Unity, inspired by GameFlow's 'Parameters' component. An AnyParameterList component can hold any set of values of different types. This library can be used without writing any additional scripts.

## Usage

- holding values for AssetBundle (especially for iOS AssetBundle, where an AssetBundle can't hold new scripts)
- etc?

## Features

- Unity's serialization system friendly (not using original serialization code). So this library can be used in scenes, AssetBundles, prefabs, etc.
- a parameter can have references of objects which is located in the scene hierarchy, like usual MonoBehavior subclasses.
- parameters can be manipulated easily on Unity Editor GUI.
- supports undo, copying component values.
- small code base


## How to install

Place the code anywhere under your Unity project folder's Assets folder.

example:

```sh
$ cd Assets
$ git clone https://github.com/showcase-tv/AnyParameterListForUnity.git
```

## Environment

tested with Unity 5.6.2p1/Mac OS X Sierra

## What is AnyParameterList class

- AnyParameterList is a component (inherits MonoBehavior).
- An AnyParameterList instance can have an array of AnyParameter's. 
- An AnyParameterList instance can have any set of multiple AnyParameter instances of any types.

## What is AnyParameter class

- AnyParameter is also a component (MonoBehavior), but be used only inside AnyParameterList components. An AnyParameter instance have a value of any supported types.

### AnyParameter.id

id (name) of this parameter

### AnyParameter.typeName

name of currently having type

### AnyParameter.boolValue, intValue, objectValue, ...

the value of this parameter. You need to choose correct value property, depends on the type of the parameter.

## Drawbacks

- It uses more storage spaces than ideal, because one AnyParameter instance actually have all of the values of supported native types, except inherited types from Object.

## Currently supported value types

- Boolean
- Int (Int32)
- Double
- String
- Vector2
- Vector3
- UnityEngine.Object
- UnityEngine.GameObject
- UnityEngine.Texture2D

## License

MIT license

