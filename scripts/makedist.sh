#!/bin/bash

if [ ! -d src/FlappyBlazorBird/FlappyBlazorBird.ClientWasm ]; then
   echo "check dirs"
   exit -1
fi

dotnet publish -c release src/FlappyBlazorBird/FlappyBlazorBird.ClientWasm
rm -ifr ./dist
mkdir ./dist
mv ./src/FlappyBlazorBird/FlappyBlazorBird.ClientWasm/bin/release/net5.0/publish ./dist

( cd dist/publish; scp -r * $USER@flappyblazorbird.ctrl-alt-d.net:/home/flappy/apps/flappyblazorbird )

echo "done"