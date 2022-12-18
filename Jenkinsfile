pipeline {
    agent any
        
    stages {
        stage('Dependencies') {
            steps {
                sh 'steamcmd +force_install_dir \$HOME/scpsl +login anonymous +app_update 996560 -beta pluginapi-beta validate +quit'
                sh 'ln -s "\$HOME/scpsl/SCPSL_Data/Managed" ".scpsl_libs"'
                sh 'cd SCPDiscordBot; dotnet restore'
                sh 'cd SCPDiscordPlugin; nuget restore -SolutionDirectory .'
            }
        }
        stage('Build') {
            parallel {
                stage('Plugin') {
                    steps {
                        sh 'msbuild SCPDiscordPlugin/SCPDiscordPlugin.csproj -p:PostBuildEvent='
                    }
                }
                stage('Bot') {
                    steps {
                        dir(path: 'SCPDiscordBot') {
                            sh 'dotnet build --output bin/linux-x64 --configuration Release --runtime linux-x64'
                            sh 'dotnet build --output bin/win-x64 --configuration Release --runtime win-x64'
                        }
                    }
                }
            }
        }
        stage('Setup Output Dir') {
            steps {
                sh 'mkdir dependencies'
                sh 'mkdir bot'
            }
        }
        stage('Package') {
            parallel {
                stage('Plugin') {
                    steps {
                        sh 'mv SCPDiscordPlugin/bin/SCPDiscord.dll ./'
                        sh 'mv SCPDiscordPlugin/bin/YamlDotNet.dll dependencies'
                        sh 'mv SCPDiscordPlugin/bin/Google.Protobuf.dll dependencies'
                        sh 'mv SCPDiscordPlugin/bin/Newtonsoft.Json.dll dependencies'
                    }
                }
                stage('Bot') {
                    steps {
                        dir(path: 'SCPDiscordBot') {
                            sh 'warp-packer --arch linux-x64 --input_dir bin/linux-x64 --exec SCPDiscordBot --output ../SCPDiscordBot_Linux'
                            sh 'warp-packer --arch windows-x64 --input_dir bin/win-x64 --exec SCPDiscordBot.exe --output ../SCPDiscordBot_Windows.exe'
                        }
                    }
                }
            }
        }
        stage('Archive') {
            steps {
                sh 'zip -r dependencies.zip dependencies'
                archiveArtifacts(artifacts: 'dependencies.zip', onlyIfSuccessful: true)
                archiveArtifacts(artifacts: 'SCPDiscord.dll', onlyIfSuccessful: true)
                archiveArtifacts(artifacts: 'SCPDiscordBot_Linux', onlyIfSuccessful: true)
                archiveArtifacts(artifacts: 'SCPDiscordBot_Windows.exe', onlyIfSuccessful: true)
            }
        }
    }
}
