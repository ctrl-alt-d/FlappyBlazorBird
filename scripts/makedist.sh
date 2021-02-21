#!/bin/bash

if [ ! -d src/FlappyBlazorBird/FlappyBlazorBird.ClientWasm ]; then
   echo "check dirs"
   exit -1
fi

dotnet publish -c release src/FlappyBlazorBird/FlappyBlazorBird.ClientWasm
rm -ifr ./dist
mkdir ./dist
mv ./src/FlappyBlazorBird/FlappyBlazorBird.ClientWasm/bin/release/net6.0/publish ./dist

( cd dist/publish/wwwroot; scp -r * $SSHUSER@flappyblazorbird.ctrl-alt-d.net:/home/flappy/apps/flappyblazorbird )

echo "done"