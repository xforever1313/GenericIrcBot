@Library( "X13JenkinsLib" )_

def ParseTestResults( String filePattern )
{
    def results = xunit thresholds: [
        failed(failureNewThreshold: '0', failureThreshold: '0', unstableNewThreshold: '0', unstableThreshold: '0')
    ], tools: [
        MSTest(
            deleteOutputFiles: true,
            failIfNotNew: true,
            pattern: filePattern,
            skipNoTestFiles: true,
            stopProcessingIfError: true
        )
    ]
}

def RunCommand( String cmd )
{
    if( isUnix() )
    {
        sh cmd;
    }
    else
    {
        bat cmd;
    }
}

def CallCake( String arguments )
{
    RunCommand( "./Cake/dotnet-cake ./checkout/build.cake ${arguments}" );
}

def CallDevops( String arguments )
{
    RunCommand( "dotnet ./checkout/Chaskis/Chaskis/DevOps/DevOps.dll ${arguments}" );
}

def Prepare()
{
    RunCommand( 'dotnet tool update Cake.Tool --tool-path ./Cake' )
    CallCake( "--showdescription" )
}

def Build()
{
    CallCake( "--target=build" );
}

def RunUnitTests()
{
    CallDevops( "--target=unit_test" );
}

pipeline
{
    agent none
    environment
    {
        DOTNET_CLI_TELEMETRY_OPTOUT = 'true'
        DOTNET_NOLOGO = 'true'
    }
    parameters
    {
        booleanParam( name: "BuildWindows", defaultValue: true, description: "Should we build for Windows?" );
        booleanParam( name: "BuildLinux", defaultValue: true, description: "Should we build for Linux?" );
        booleanParam( name: "RunUnitTests", defaultValue: true, description: "Should unit tests be run?" );
        booleanParam( name: "RunRegressionTests", defaultValue: true, description: "Should regression tests be run?" );
    }
    options
    {
        skipDefaultCheckout( true );
    }
    stages
    {
        stage( 'Build & Test' )
        {
            parallel
            {
                stage( 'Windows' )
                {
                    agent
                    {
                        label "windows && docker && x64";
                    }
                    stages
                    {
                        stage( 'Enable Docker' )
                        {
                            steps
                            {
                                bat 'C:\\"Program Files"\\Docker\\Docker\\DockerCli.exe -Version';
                                bat 'C:\\"Program Files"\\Docker\\Docker\\DockerCli.exe -SwitchWindowsEngine';
                            }
                        }
                        stage( 'clean' )
                        {
                            steps
                            {
                                cleanWs();
                            }
                        }
                        stage( 'checkout' )
                        {
                            steps
                            {
                                // TODO: put this back in once configured in Jenkins
                                // checkout scm;
                                checkout([$class: 'GitSCM', branches: [[name: '*/master']], extensions: [[$class: 'RelativeTargetDirectory', relativeTargetDir: 'checkout'], [$class: 'CleanCheckout'], [$class: 'SubmoduleOption', disableSubmodules: false, parentCredentials: false, recursiveSubmodules: true, reference: '', trackingSubmodules: false]], userRemoteConfigs: [[url: 'https://github.com/xforever1313/Chaskis.git']]])
                            }
                        }
                        stage( 'In Docker' )
                        {
                            agent
                            {
                                docker
                                {
                                    image 'mcr.microsoft.com/dotnet/sdk:3.1'
                                    args "-e HOME='${env.WORKSPACE}'"
                                    reuseNode true
                                }
                            }
                            stages
                            {
                                stage( 'prepare' )
                                {
                                    steps
                                    {
                                        Prepare();
                                    }
                                }
                                stage( 'build' )
                                {
                                    steps
                                    {
                                        Build();
                                    }
                                }
                                stage( 'unit_test' )
                                {
                                    steps
                                    {
                                        RunUnitTests();
                                    }
                                    when
                                    {
                                        expression
                                        {
                                            return params.RunUnitTests;
                                        }
                                    }
                                    post
                                    {
                                        always
                                        {
                                            ParseTestResults( "checkout/TestResults/UnitTests/*.xml" );
                                        }
                                    }
                                }
                            }
                        }
                    }
                    when
                    {
                        expression
                        {
                            return params.BuildWindows;
                        }
                    }
                }

                stage( 'Linux' )
                {
                    agent
                    {
                        label "ubuntu && docker && x64";
                    }
                    stages
                    {
                        stage( 'clean' )
                        {
                            steps
                            {
                                cleanWs();
                            }
                        }
                        stage( 'checkout' )
                        {
                            steps
                            {
                                // TODO: put this back in once configured in Jenkins
                                // checkout scm;
                                checkout([$class: 'GitSCM', branches: [[name: '*/master']], extensions: [[$class: 'RelativeTargetDirectory', relativeTargetDir: 'checkout'], [$class: 'CleanCheckout'], [$class: 'SubmoduleOption', disableSubmodules: false, parentCredentials: false, recursiveSubmodules: true, reference: '', trackingSubmodules: false]], userRemoteConfigs: [[url: 'https://github.com/xforever1313/Chaskis.git']]])
                            }
                        }
                        stage( 'In Docker' )
                        {
                            agent
                            {
                                docker
                                {
                                    image 'mcr.microsoft.com/dotnet/sdk:3.1'
                                    args "-e HOME='${env.WORKSPACE}'"
                                    reuseNode true
                                }
                            }
                            stages
                            {
                                stage( 'prepare' )
                                {
                                    steps
                                    {
                                        Prepare();
                                    }
                                }
                                stage( 'build' )
                                {
                                    steps
                                    {
                                        Build();
                                    }
                                }
                                stage( 'unit_test' )
                                {
                                    steps
                                    {
                                        RunUnitTests();
                                    }
                                    when
                                    {
                                        expression
                                        {
                                            return params.RunUnitTests;
                                        }
                                    }
                                    post
                                    {
                                        always
                                        {
                                            ParseTestResults( "checkout/TestResults/UnitTests/*.xml" );
                                        }
                                    }
                                }
                            }
                        }
                    }
                    when
                    {
                        expression
                        {
                            return params.BuildLinux;
                        }
                    }
                }
            }
        }
    }
}