#!/bin/bash

if [ ! -d src/FlappyBlazorBird/FlappyBlazorBird.Client ]; then
   echo "check dirs"
   exit -1
fi

dotnet publish -c release src/FlappyBlazorBird/FlappyBlazorBird.Client
rm -ifr ./dist
mkdir ./dist
mv ./src/FlappyBlazorBird/FlappyBlazorBird.Client/bin/release/netstandard2.1/publish ./dist

echo "done"